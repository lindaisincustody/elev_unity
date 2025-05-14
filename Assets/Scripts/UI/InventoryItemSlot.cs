using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryItemSlot : MonoBehaviour,
                                IPointerEnterHandler,
                                IPointerExitHandler,
                                IPointerClickHandler
{
    public TextMeshProUGUI itemCount;
    public Image border;
    public Image image;
    public Sprite pass_parts_3;

    // this will be set by your UI when you populate
    [HideInInspector] public int slotIndex;

    private Item currentItem;
    public bool isEquiped;

    // parent UI will subscribe to this
    public Action<int> OnClick;

    public void Equip(Item item, int count)
    {
        currentItem = item;
        image.sprite = item.sprite;
        isEquiped = true;

        if (count > 1)
        {
            itemCount.text = count.ToString();
            itemCount.gameObject.SetActive(true);
        }
        else
        {
            itemCount.gameObject.SetActive(false);
        }
    }

    public Item GetItem() => isEquiped ? currentItem : null;

    public void Clear()
    {
        currentItem = null;
        image.sprite = pass_parts_3;
        isEquiped = false;
        itemCount.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData e) => border.color = new Color(1, 1, 1, 0.3f);
    public void OnPointerExit(PointerEventData e) => border.color = Color.white;

    public void OnPointerClick(PointerEventData e)
    {
        // only fire if there's a subscriber
        OnClick?.Invoke(slotIndex);
    }
}
