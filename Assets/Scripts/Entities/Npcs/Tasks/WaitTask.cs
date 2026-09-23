using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/Wait")]
public class WaitTask : NpcTask
{
    public float minDuration = 1f;
    public float maxDuration = 3f;

    public override async UniTask Run(NpcContext context, CancellationToken token)
    {
        context.movement.target = null;
        context.npc.Get<NpcAnimator>().Play(NpcAnimator.AnimationType.Idle);

        float duration = UnityEngine.Random.Range(minDuration, maxDuration);

        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
    }
}
