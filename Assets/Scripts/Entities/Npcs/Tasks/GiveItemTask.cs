using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/GiveItem")]
public class GiveItemTask : NpcTask
{
    public Item item;

    public override UniTask Run(NpcContext context, CancellationToken token)
    {
        context.movement.FaceTarget(context.player);
        ItemDropper.Instance.Drop(item, context.npc.transform.position);

        return UniTask.CompletedTask;
    }
}
