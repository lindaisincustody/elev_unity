using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public List<Item> items = new List<Item>();
}

public class ItemsInventory : Component
{
    public InventoryData inventoryData = new InventoryData();

    private SavingWrapper savingWrapper;


    private void Awake()
    {
        savingWrapper = SavingWrapper.Instance;
        inventoryData = savingWrapper.LoadInventory();
    }

    public void AddItem(Item newItem)
    {
        inventoryData.items.Add(newItem);
        savingWrapper.SaveInventory(inventoryData);
    }

    public void DeleteItems()
    {
        inventoryData.items.Clear();
        savingWrapper.SaveInventory(inventoryData);
    }

    public void RemoveItem(Item itemToRemove)
    {
        // Check if the item exists in the inventory
        if (inventoryData.items.Contains(itemToRemove))
        {
            inventoryData.items.Remove(itemToRemove);
            savingWrapper.SaveInventory(inventoryData); // Save changes
        }
        else
        {
            Debug.LogWarning("Attempted to remove an item not present in the inventory.");
        }
    }

    public List<Item> GetAllItems()
    {
        return inventoryData.items;
    }
}
