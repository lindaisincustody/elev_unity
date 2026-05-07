using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
// Unity.Services.Multiplayer re-exports the Relay types in the same namespace,
// so Unity.Services.Relay must be removed from the Package Manager to avoid CS0433.
// These usings are provided by the Multiplayer package.
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// Bootstraps Unity Gaming Services and owns the Relay host/join flow.
/// Place on the same GameObject as Unity's NetworkManager.
///
/// REQUIRED SETUP (one time):
///   1. Edit → Project Settings → Services → link/create a Unity project
///   2. dashboard.unity3d.com → your project → Relay → Enable
///   3. dashboard.unity3d.com → your project → Authentication → Enable
/// </summary>
public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager Instance { get; private set; }

    /// <summary>The 6-character code shown to the host after CreateAllocation.</summary>
    public string JoinCode { get; private set; }

    private bool _ugsReady = false;
    private Task _initTask;   // cached so parallel callers don't double-init

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Lazy initialisation ───────────────────────────────────────────────

    /// <summary>
    /// Initialises UGS and signs in anonymously.
    /// Safe to await multiple times — only runs once.
    /// </summary>
    private async Task EnsureUGSReadyAsync()
    {
        if (_ugsReady) return;

        // Only start the init task once; parallel callers await the same task.
        if (_initTask == null)
            _initTask = InitUGSAsync();

        await _initTask;
    }

    private async Task InitUGSAsync()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _ugsReady = true;
        Debug.Log($"[Net] UGS ready — player ID: {AuthenticationService.Instance.PlayerId}");
    }

    // ── Host ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Initialises UGS if needed, allocates a Relay slot for 2 players,
    /// starts a host, and returns the 6-char join code.  Returns null on failure.
    /// </summary>
    public async Task<string> HostAsync()
    {
        try
        {
            await EnsureUGSReadyAsync();

            // maxConnections = 1 → 1 additional client (2 players total)
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(1);
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                alloc.RelayServer.IpV4,
                (ushort)alloc.RelayServer.Port,
                alloc.AllocationIdBytes,
                alloc.Key,
                alloc.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            Debug.Log($"[Net] Hosting — join code: {JoinCode}");
            return JoinCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Net] Host failed: {e.Message}\n{e.InnerException?.Message}");
            return null;
        }
    }

    // ── Join ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Initialises UGS if needed, then joins an existing Relay session by code.
    /// Returns true on success.
    /// </summary>
    public async Task<bool> JoinAsync(string code)
    {
        try
        {
            await EnsureUGSReadyAsync();

            JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(
                code.Trim().ToUpper());

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                alloc.RelayServer.IpV4,
                (ushort)alloc.RelayServer.Port,
                alloc.AllocationIdBytes,
                alloc.Key,
                alloc.ConnectionData,
                alloc.HostConnectionData   // tells the transport where the host is
            );

            NetworkManager.Singleton.StartClient();
            Debug.Log("[Net] Joined as client.");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Net] Join failed: {e.Message}\n{e.InnerException?.Message}");
            return false;
        }
    }

    // ── Disconnect ────────────────────────────────────────────────────────

    public void Disconnect()
    {
        NetworkManager.Singleton?.Shutdown();
    }
}
