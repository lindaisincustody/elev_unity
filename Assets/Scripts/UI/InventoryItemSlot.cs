using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour
{
    public Image image;
    public Sprite pass_parts_3;
    private Item currentItem;
    public bool isEquiped;

    public int slotIndex;

    public void Equip(Item item)
    {
        currentItem = item;
        image.sprite = item.sprite;
        isEquiped = true;
    }

    public Item GetItem()
    {
        return isEquiped ? currentItem : null;
    }

    public void Clear()
    {
        currentItem = null;
        image.sprite = pass_parts_3;
        isEquiped = false;
    }

    public bool IsFree()
    {
        return !isEquiped;
    }
}