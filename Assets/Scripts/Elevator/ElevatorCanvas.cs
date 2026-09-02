using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ElevatorCanvas : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private ElevatorManager elevatorManager;

    private InputManager playerInput;
    private CancellationTokenSource rideCancellation;

    private void Awake()
    {
        playerInput = InputManager.Instance;
        playerInput.OnUICancel += Close;

        rideCancellation = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        playerInput.OnUICancel -= Close;

        rideCancellation.Cancel();
        rideCancellation.Dispose();
    }

    public async UniTask<bool> Ride(NPCData passenger, int miniGameLevels)
    {
        if (!UIManager.Instance.RequestOpen(this))
            return false;

        panel.SetActive(true);
        Player.instance.SetMovement(false);

        rideCancellation.Dispose();
        rideCancellation = new CancellationTokenSource();

        bool canceled = await elevatorManager
            .Ride(passenger, miniGameLevels, rideCancellation.Token)
            .SuppressCancellationThrow();

        return !canceled;
    }

    public void Close()
    {
        if (!panel.activeSelf)
            return;

        rideCancellation.Cancel();
        elevatorManager.Stop();

        panel.SetActive(false);
        Player.instance.SetMovement(true);
        UIManager.Instance.NotifyClosed(this);
    }
}
