using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles spawning Player 2 when a client connects.
/// Plain MonoBehaviour — safe to put on the NetworkManager GameObject.
///
/// SETUP:
///   1. In the NetworkManager inspector set "Default Player Prefab" to NONE.
///   2. Assign playerTwoPrefab — the Player prefab with NetworkObject +
///      NetworkTransform + NetworkPlayerSync (P2 animator assigned).
///   3. Assign playerTwoSpawnPoint — empty GO where P2 should appear.
///   4. Add the Player prefab to NetworkManager's Network Prefabs list.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Player 2")]
    [Tooltip("Player prefab with NetworkObject + NetworkTransform + NetworkPlayerSync.")]
    [SerializeField] private GameObject playerTwoPrefab;

    [Tooltip("Where Player 2 spawns when they join.")]
    [SerializeField] private Transform playerTwoSpawnPoint;

    private void Start()
    {
        // Wait until the server is fully started before listening for clients.
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnServerStarted             -= OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback   -= OnClientConnected;
    }

    private void OnServerStarted()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only the server spawns objects.
        if (!NetworkManager.Singleton.IsServer) return;

        // Host's own player is already in the scene — skip.
        if (clientId == NetworkManager.ServerClientId) return;

        if (playerTwoPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] playerTwoPrefab is not assigned!");
            return;
        }

        // Prefer an explicit spawn point; fall back to beside P1 so P2 is always on screen.
        Vector3 spawnPos;
        if (playerTwoSpawnPoint != null)
            spawnPos = playerTwoSpawnPoint.position;
        else if (Player.instance != null)
            spawnPos = Player.instance.transform.position + new Vector3(2f, 0f, 0f);
        else
            spawnPos = Vector3.zero;

        var go     = Instantiate(playerTwoPrefab, spawnPos, Quaternion.identity);
        var netObj = go.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId);

        Debug.Log($"[PlayerSpawner] Spawned Player 2 for client {clientId}");
    }
}
