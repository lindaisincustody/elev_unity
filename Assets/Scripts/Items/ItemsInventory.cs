using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public List<ShopItem> items = new List<ShopItem>();
}

public class ItemsInventory : MonoBehaviour
{
    public InventoryData inventoryData = new InventoryData();

    private SavingWrapper savingWrapper;
    private static ItemsInventory _instance;
    public static ItemsInventory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemsInventory>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("ItemsInventory");
                    _instance = singletonObject.AddComponent<ItemsInventory>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        savingWrapper = SavingWrapper.Instance;
        inventoryData = savingWrapper.LoadInventory();

    }

    public void AddItem(ShopItem newItem)
    {
        inventoryData.items.Add(newItem);
        savingWrapper.SaveInventory(inventoryData);
    }

    public void DeleteItems()
    {
        inventoryData.items.Clear();
        savingWrapper.SaveInventory(inventoryData);
    }

    public List<ShopItem> GetAllItems()
    {
        return inventoryData.items;
    }
}
