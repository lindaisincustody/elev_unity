using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Item _item;

    public void SetItem(Item item)
    {
        _item = item;

        spriteRenderer.sprite = item.sprite;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Player.instance.Get<ItemsInventory>().AddItem(_item);
        Destroy(gameObject);
    }
}
