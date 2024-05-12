using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour
{
    public Image image;
    public Sprite pass_parts_3;
    private ShopItem currentItem; // Store the current item

    public bool isEquiped;

    public void Equip(ShopItem item)
    {
        currentItem = item;
        image.sprite = item.sprite;
        isEquiped = true;
    }

    public ShopItem GetItem()
    {
        return isEquiped ? currentItem : null; 
    }
    public void Clear()
    {
        currentItem = null; // Clear the stored item
        image.sprite = pass_parts_3; // Remove the sprite
        isEquiped = false;
    }

    public bool IsFree()
    {
        return !isEquiped;
    }
}
