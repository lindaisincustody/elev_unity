using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Tasks/MoveToAnchor")]
public class MoveToAnchorTask : NpcTask
{
    public string anchorId;

    public override async UniTask Run(NpcContext context, CancellationToken token)
    {
        Transform anchor = NpcManager.Instance.GetClosestAnchor(anchorId, context.npc.transform.position);

        await context.movement.MoveTo(anchor.position, token);
    }
}
