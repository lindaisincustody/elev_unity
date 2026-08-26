using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerFollowCamera : MonoBehaviour
{
    public static PlayerFollowCamera Instance { get; private set; }

    private CinemachineVirtualCamera vcam;

    private void Awake()
    {
        Instance = this;
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        GameSession.Instance.OnGameStarted += Follow;

        if (GameSession.Instance.IsRunning)
            Follow();
    }

    private void OnDestroy()
    {
        GameSession.Instance.OnGameStarted -= Follow;
    }

    public void Snap()
    {
        vcam.PreviousStateIsValid = false;
    }

    private void Follow()
    {
        vcam.Follow = Player.instance.transform;
    }
}
