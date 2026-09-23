using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Triggers/Underworld")]
public class UnderworldTrigger : NpcTrigger
{
    public bool inUnderworld = true;

    public override bool IsMet(NpcContext context)
    {
        return SanityManager.Instance.IsPlayerInUnderworld == inUnderworld;
    }
}
