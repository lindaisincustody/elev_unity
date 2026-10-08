using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private const string OpenClip = "ElevatorOpen";
    private const string CloseClip = "ElevatorClose";

    [SerializeField] private Animation doors;
    [SerializeField] private Transform front;
    [SerializeField] private Transform inside;
    [SerializeField] private float closeDelay = 1f;
    [SerializeField] private Vector2 shakeAmplitude = new Vector2(0.012f, 0.008f);
    [SerializeField] private float shakeFrequency = 10f;
    [SerializeField] private float shakeDuration = 0.6f;
    [SerializeField] private float liftHeight = 3f;
    [SerializeField] private float liftSpeed = 1.5f;

    private void OnEnable()
    {
        ElevatorGameManager.Instance.RegisterElevator(this);
    }

    private void OnDisable()
    {
        ElevatorGameManager.Instance.UnregisterElevator(this);
    }

    public async UniTask Board(NpcContext context, CancellationToken token)
    {
        await context.movement.MoveTo(front.position, token);
        await PlayDoors(OpenClip, token);
        context.npc.Get<NpcSorting>().ChangeLayer();
        await context.movement.WalkInto(inside.position, token);
        await UniTask.Delay(TimeSpan.FromSeconds(closeDelay), cancellationToken: token);
        await PlayDoors(CloseClip, token);
        context.npc.Get<NpcSorting>().HideOutsideMask();
        await context.movement.Shake(shakeAmplitude, shakeFrequency, shakeDuration, token);
        await context.movement.Glide(inside.position + Vector3.up * liftHeight, liftSpeed, token);

        Destroy(context.npc.gameObject);
        ResetDoors();

        ElevatorGameManager.Instance.NotifyPassengerLifted();
    }

    private void ResetDoors()
    {
        doors.Stop();
    }

    private UniTask PlayDoors(string clip, CancellationToken token)
    {
        doors.Play(clip);

        return UniTask.Delay(TimeSpan.FromSeconds(doors[clip].length), cancellationToken: token);
    }
}
