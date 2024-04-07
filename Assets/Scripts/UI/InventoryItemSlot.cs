using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour
{
    public Image image;
    public bool isEquiped;

    public void Equip(ShopItem item)
    {
        image.sprite = item.sprite;
        isEquiped = true;
    }

    public bool IsFree()
    {
        return !isEquiped;
    }
}
