using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/FollowPlayer")]
public class FollowPlayerTask : NpcTask
{
    public float stopDistance = 2f;
    public float duration = 6f;

    public override async UniTask Run(NpcContext context, CancellationToken token)
    {
        float deadline = Time.time + duration;

        while (Time.time < deadline)
        {
            float distance = Vector2.Distance(context.npc.transform.position, context.player.position);

            context.movement.target = distance > stopDistance ? (Vector2?)context.player.position : null;

            await UniTask.Yield(token);
        }

        context.movement.target = null;
    }
}
