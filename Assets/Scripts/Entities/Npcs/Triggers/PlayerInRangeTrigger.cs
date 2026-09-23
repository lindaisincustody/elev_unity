using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Triggers/PlayerInRange")]
public class PlayerInRangeTrigger : NpcTrigger
{
    public float range = 4f;

    public override bool IsMet(NpcContext context)
    {
        return Vector2.Distance(context.npc.transform.position, context.player.position) <= range;
    }
}
