using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Triggers/PlayerHasItem")]
public class PlayerHasItemTrigger : NpcTrigger
{
    public Item item;

    public override bool IsMet(NpcContext context)
    {
        return Player.instance.Get<ItemsInventory>().GetAllItems().Contains(item);
    }
}
