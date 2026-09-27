using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/RideElevator")]
public class RideElevatorTask : NpcTask
{
    public override async UniTask Run(NpcContext context, CancellationToken token)
    {
        context.movement.target = null;
        context.npc.Get<NpcAnimator>().Play(NpcAnimator.AnimationType.Idle);

        await ElevatorGameManager.Instance.WaitForRide(context.npc, token);
    }
}
