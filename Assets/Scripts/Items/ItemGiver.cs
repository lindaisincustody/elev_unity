using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] private Item dashItem;
    [SerializeField] private Item trapItem;
    [SerializeField] private Item stunChanceItem;

    [Button]
    public void GiveDash()
    {
        Player.instance.Get<ItemsInventory>().AddItem(dashItem);
    }

    [Button]
    public void GiveTrap()
    {
        Player.instance.Get<ItemsInventory>().AddItem(trapItem);
    }

    [Button]
    public void GiveStunChance()
    {
        Player.instance.Get<ItemsInventory>().AddItem(stunChanceItem);
    }
}
