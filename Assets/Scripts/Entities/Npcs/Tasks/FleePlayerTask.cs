using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/FleePlayer")]
public class FleePlayerTask : NpcTask
{
    public float distance = 6f;
    public float duration = 3f;

    public override async UniTask Run(NpcContext context, CancellationToken token)
    {
        float deadline = Time.time + duration;

        while (Time.time < deadline)
        {
            Vector2 position = context.npc.transform.position;
            Vector2 away = (position - (Vector2)context.player.position).normalized;

            context.movement.target = position + away * distance;

            await UniTask.Yield(token);
        }

        context.movement.target = null;
    }
}
