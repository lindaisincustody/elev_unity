using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item Item;
    public TextMeshProUGUI itemCount;
    public Image border;
    public Image image;
    public Sprite pass_parts_3;

    [HideInInspector] public int slotIndex;

    public Action<int> OnClick;

    public void Equip(Item item, int count)
    {
        Item = item;
        image.sprite = item.sprite;

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

    public void Clear()
    {
        Item = null;
        image.sprite = pass_parts_3;
        itemCount.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData e) => border.color = new Color(1, 1, 1, 0.3f);
    public void OnPointerExit(PointerEventData e) => border.color = Color.white;

    public void OnPointerClick(PointerEventData e)
    {
        OnClick?.Invoke(slotIndex);
    }
}
