using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/TakeItem")]
public class TakeItemTask : NpcTask
{
    public Item item;

    public override UniTask Run(NpcContext context, CancellationToken token)
    {
        context.movement.FaceTarget(context.player);
        Player.instance.Get<ItemsInventory>().RemoveItem(item);

        return UniTask.CompletedTask;
    }
}
