using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        var items = ItemsInventory.Instance.GetAllItems();

        foreach (var item in items)
        {
            item.item.OnGameStart();
        }
    }
}
