using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] private Item dashItem;
    [SerializeField] private Item trapItem;

    [Button]
    public void GiveDash()
    {
        Player.instance.ItemsInventory.AddItem(dashItem);
    }

    [Button]
    public void GiveTrap()
    {
        Player.instance.ItemsInventory.AddItem(trapItem);
    }
}
