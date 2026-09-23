using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/PlayAnimation")]
public class PlayAnimationTask : NpcTask
{
    public NpcAnimator.AnimationType animation = NpcAnimator.AnimationType.Interact;
    public float duration = 2f;

    public override async UniTask Run(NpcContext context, CancellationToken token)
    {
        context.movement.target = null;
        context.npc.Get<NpcAnimator>().Play(animation);

        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);

        context.npc.Get<NpcAnimator>().Play(NpcAnimator.AnimationType.Idle);
    }
}
