using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Add this to the Player prefab alongside NetworkObject + NetworkTransform.
///
/// Responsibilities:
///   - Assigns Player.instance to the local owner so all existing single-player
///     code (camera follow, ability registry, etc.) keeps working unchanged.
///   - Disables input and drawing on remote-player proxies so Phone A's touches
///     never drive Phone B's proxy character.
///   - Swaps the Animator controller (and optional sprite tint) for Player 2
///     so both players have distinct looks.
///   - Relays ML drawing results to the other device via ServerRpc → ClientRpc.
///   - Syncs HP via a NetworkVariable so both screens show correct health bars.
/// </summary>
[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayerSync : NetworkBehaviour
{
    // ── Inspector: Player 2 visuals ───────────────────────────────────────

    [Header("Player 2 Visuals")]
    [Tooltip("Animator Controller with Player 2's idle/walk/dash animations. " +
             "Assigned automatically on the remote proxy and on P2's own device.")]
    [SerializeField] private RuntimeAnimatorController playerTwoAnimator;

    [Tooltip("Optional: tint the SpriteRenderer for Player 2 " +
             "(white = no tint). Useful if you share the same sprites but want a colour difference).")]
    [SerializeField] private Color playerTwoColor = Color.white;

    // ── Synced state ──────────────────────────────────────────────────────

    /// <summary>Owner writes, everyone reads.</summary>
    public NetworkVariable<int> SyncedHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    /// <summary>
    /// Movement vector synced every frame from the owner so remote proxies
    /// can drive their Animator without running PlayerMovement locally.
    /// </summary>
    public NetworkVariable<Vector2> SyncedMovement = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    // ── Static convenience accessor ───────────────────────────────────────

    /// <summary>The NetworkPlayerSync that belongs to this device's local player.</summary>
    public static NetworkPlayerSync Local { get; private set; }

    // ── Cached component refs ─────────────────────────────────────────────

    private Player         player;
    private PlayerMovement playerMovement;
    private LetterDrawing  letterDrawing;
    private Animator       proxyAnimator;   // animator on the remote proxy

    // ── Spawn / despawn ───────────────────────────────────────────────────

    public override void OnNetworkSpawn()
    {
        player         = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();

        // LetterDrawing may live on the same GO or a child — find it either way.
        letterDrawing = GetComponentInChildren<LetterDrawing>(includeInactive: true);
        if (letterDrawing == null)
            letterDrawing = FindObjectOfType<LetterDrawing>(); // scene-level fallback

        // ── Determine player slot ─────────────────────────────────────────
        // Host always has ServerClientId (0); the joining client gets a higher ID.
        // We use this to decide visuals — no extra NetworkVariable needed because
        // OwnerClientId is automatically synchronised by NGO.
        bool isPlayerTwo = OwnerClientId != NetworkManager.ServerClientId;
        if (isPlayerTwo)
            ApplyPlayerTwoVisuals();

        // ── Local vs remote setup ─────────────────────────────────────────
        if (IsOwner)
        {
            // LOCAL player: point the singleton so all existing code works.
            Local = this;
            Player.instance = player;

            // Camera scripts cache Player.instance.transform in their Start/Awake,
            // which runs before network objects spawn. Re-point them now so P2's
            // camera follows their own character instead of P1's proxy.
            var smoothCam = FindObjectOfType<SmoothCameraFollow>();
            if (smoothCam != null) smoothCam.SetPlayer(transform);

            var cineCam = FindObjectOfType<CinemachineTopDownAim_CM2>();
            if (cineCam != null) cineCam.SetPlayer(transform);

            // Wire the scene joystick to this player's InputManager so P2 can move.
            // P1 is already wired via the Inspector; calling SetJoystick on P1 is harmless.
            var joystick = FindObjectOfType<TouchJoystick>();
            var inputMgr = GetComponent<InputManager>();
            if (joystick != null && inputMgr != null)
                inputMgr.SetJoystick(joystick);

            Debug.Log($"[Net] Local player spawned (slot: {(isPlayerTwo ? "P2" : "P1")}).");
        }
        else
        {
            // REMOTE proxy: disable input so this device can't drive the other player.
            if (playerMovement != null)
                playerMovement.enabled = false;

            // Disable drawing only if LetterDrawing lives on the Player GO itself.
            // If it's a separate scene object it already belongs to the local player.
            if (letterDrawing != null && letterDrawing.gameObject == player.gameObject)
                letterDrawing.enabled = false;

            // Cache the animator so we can drive it from SyncedMovement changes.
            proxyAnimator = GetComponent<Animator>();
            if (proxyAnimator == null) proxyAnimator = GetComponentInChildren<Animator>();
            SyncedMovement.OnValueChanged += OnSyncedMovementChanged;

            Debug.Log($"[Net] Remote proxy spawned (slot: {(isPlayerTwo ? "P2" : "P1")}).");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) Local = null;
        else SyncedMovement.OnValueChanged -= OnSyncedMovementChanged;
    }

    // ── Animation sync ────────────────────────────────────────────────────

    private void Update()
    {
        if (!IsOwner || playerMovement == null) return;

        // Write movement every frame so remote proxies can animate.
        // Only write when the value actually changes to save bandwidth.
        Vector2 move = playerMovement.movement;
        if (SyncedMovement.Value != move)
            SyncedMovement.Value = move;
    }

    /// <summary>Fires on every device that has a remote proxy for this player.</summary>
    private void OnSyncedMovementChanged(Vector2 previous, Vector2 current)
    {
        if (proxyAnimator == null) return;
        proxyAnimator.SetFloat("Horizontal", current.x);
        proxyAnimator.SetFloat("Vertical",   current.y);
        proxyAnimator.SetFloat("Speed",       current.sqrMagnitude);
    }

    // ── Visuals ───────────────────────────────────────────────────────────

    /// <summary>
    /// Swaps the Animator controller and optionally tints the sprite for Player 2.
    /// Runs on BOTH devices for the P2 object, so every screen shows the right look.
    /// </summary>
    private void ApplyPlayerTwoVisuals()
    {
        // ── Animator controller ───────────────────────────────────────────
        if (playerTwoAnimator != null)
        {
            var anim = GetComponent<Animator>();
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.runtimeAnimatorController = playerTwoAnimator;
                Debug.Log($"[Net] P2 animator applied: {playerTwoAnimator.name}");
            }
            else
                Debug.LogWarning("[Net] P2 animator NOT applied — no Animator component found on player or children.");
        }
        else
            Debug.LogWarning("[Net] P2 animator NOT applied — playerTwoAnimator is not assigned in the Inspector on the Player prefab's NetworkPlayerSync.");

        // ── Sprite tint (optional) ────────────────────────────────────────
        if (playerTwoColor != Color.white)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.color = playerTwoColor;
        }
    }

    // ── Drawing result sync ───────────────────────────────────────────────

    /// <summary>
    /// Called by PredictingDrawingState after ML produces a result.
    /// Sends the label to the server which broadcasts it to all clients.
    /// </summary>
    public void BroadcastDrawingResult(string label)
    {
        if (!IsOwner) return;
        DrawingResultServerRpc(label);
    }

    [ServerRpc]
    private void DrawingResultServerRpc(string label)
    {
        DrawingResultClientRpc(label);
    }

    [ClientRpc]
    private void DrawingResultClientRpc(string label)
    {
        // Owner already applied the result locally — skip double-processing.
        if (IsOwner) return;

        // Trigger the remote player's ability and enemy checks on this device too.
        AbilityActionRegistry.Instance?.ExecuteAction(label);
        Debug.Log($"[Net] Remote player drew: {label}");
    }

    // ── Health sync ───────────────────────────────────────────────────────

    /// <summary>Call this whenever the local player's HP changes.</summary>
    public void SyncHealth(int newHp)
    {
        if (!IsOwner) return;
        SyncedHealth.Value = newHp;
    }
}
