using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] private Item item;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            GiveItem();
    }

    public void GiveItem()
    {
        Player.instance.ItemsInventory.AddItem(item);
    }
}
