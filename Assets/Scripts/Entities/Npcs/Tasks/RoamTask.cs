using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/Roam")]
public class RoamTask : NpcTask
{
    public float radius = 5f;
    public float timeout = 15f;

    public override async UniTask Run(NpcContext context, CancellationToken token)
    {
        Vector2 destination = HasArea(context)
            ? RandomPointInArea(context)
            : context.movement.spawnPos + UnityEngine.Random.insideUnitCircle * radius;

        await context.movement.MoveTo(destination, timeout, token);
    }

    private bool HasArea(NpcContext context)
    {
        return context.movement.navMin != context.movement.navMax;
    }

    private Vector2 RandomPointInArea(NpcContext context)
    {
        return new Vector2(
            UnityEngine.Random.Range(context.movement.navMin.x, context.movement.navMax.x),
            UnityEngine.Random.Range(context.movement.navMin.y, context.movement.navMax.y));
    }
}
