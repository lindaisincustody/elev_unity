using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pre-game overlay: Player 1 hosts and shares the room code;
/// Player 2 types it in and joins.
///
/// Setup in the Inspector:
///   lobbyRoot  — a Canvas panel with Host button, Join button, code input, status text
///   hudRoot    — a small always-visible HUD (connection dot + optional disconnect button)
/// </summary>
public class MultiplayerLobbyUI : MonoBehaviour
{
    [Header("Lobby panel (shown before both players connect)")]
    [SerializeField] private GameObject     lobbyRoot;
    [SerializeField] private Button         btnHost;
    [SerializeField] private Button         btnJoin;
    [SerializeField] private TMP_InputField codeInput;       // Player 2 types here
    [SerializeField] private TextMeshProUGUI statusLabel;    // "Waiting…", "Joining…", errors
    [SerializeField] private TextMeshProUGUI codeDisplay;    // shows the generated code to Player 1

    [Header("Toggle button (always visible — opens/closes the lobby panel)")]
    [SerializeField] private Button          btnToggleLobby;  // the cogwheel button

    [Header("In-game HUD (shown after both players are connected)")]
    [SerializeField] private GameObject      hudRoot;
    [SerializeField] private TextMeshProUGUI hudStatusText;  // "● P2 connected" etc.
    [SerializeField] private Button          btnDisconnect;  // optional

    // ──────────────────────────────────────────────────────────────────────

    private void Start()
    {
        lobbyRoot.SetActive(false);   // hidden by default — cogwheel opens it
        hudRoot.SetActive(false);

        if (btnToggleLobby != null)
            btnToggleLobby.onClick.AddListener(() => lobbyRoot.SetActive(!lobbyRoot.activeSelf));

        btnHost.onClick.AddListener(OnHostClicked);
        btnJoin.onClick.AddListener(OnJoinClicked);
        if (btnDisconnect != null)
            btnDisconnect.onClick.AddListener(() => GameNetworkManager.Instance.Disconnect());

        NetworkManager.Singleton.OnClientConnectedCallback  += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    // ── Button handlers ───────────────────────────────────────────────────

    private async void OnHostClicked()
    {
        SetInteractable(false);
        statusLabel.text  = "Creating room…";
        codeDisplay.text  = "";

        string code = await GameNetworkManager.Instance.HostAsync();
        if (code != null)
        {
            // Show the code large so the user can read it to Player 2
            codeDisplay.text = $"Room code:\n<size=130%><b>{code}</b></size>";
            statusLabel.text = "Waiting for Player 2…";
        }
        else
        {
            statusLabel.text = "Failed to create room.\nCheck your internet connection.";
            SetInteractable(true);
        }
    }

    private async void OnJoinClicked()
    {
        string code = codeInput.text.Trim();
        if (string.IsNullOrEmpty(code))
        {
            statusLabel.text = "Enter the room code first.";
            return;
        }

        SetInteractable(false);
        statusLabel.text = "Joining…";

        bool ok = await GameNetworkManager.Instance.JoinAsync(code);
        if (!ok)
        {
            statusLabel.text = "Could not join — check the code and try again.";
            SetInteractable(true);
        }
        // On success OnClientConnected fires and hides the lobby.
    }

    // ── Network callbacks ─────────────────────────────────────────────────

    private void OnClientConnected(ulong clientId)
    {
        // Host: wait until 2 players are present (self + 1 client).
        // Client: fires when the client itself connects — lobby can close immediately.
        bool ready = NetworkManager.Singleton.IsHost
            ? NetworkManager.Singleton.ConnectedClients.Count >= 2
            : true;

        if (!ready) return;

        lobbyRoot.SetActive(false);
        hudRoot.SetActive(true);
        hudStatusText.text = NetworkManager.Singleton.IsHost ? "● P2 connected" : "● Connected";
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (hudRoot.activeSelf)
            hudStatusText.text = "● P2 disconnected";
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private void SetInteractable(bool on)
    {
        btnHost.interactable = on;
        btnJoin.interactable = on;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback  -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
