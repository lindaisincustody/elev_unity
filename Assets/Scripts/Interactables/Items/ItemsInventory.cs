using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ItemsInventory : Component
{
    private readonly List<Item> items = new List<Item>();

    private GeneralSaveFile saveFile;

    private void Awake()
    {
        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
        LoadItems();
    }

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        SaveItems();
    }

    [Button]
    public void DeleteItems()
    {
        items.Clear();
        SaveItems();
    }

    public void RemoveItem(Item itemToRemove)
    {
        items.Remove(itemToRemove);
        SaveItems();
    }

    public List<Item> GetAllItems()
    {
        return items;
    }

    private void SaveItems()
    {
        List<string> itemIds = saveFile.InventorySnapshot.ItemIds;
        itemIds.Clear();

        foreach (Item item in items)
            itemIds.Add(item.itemId);

        SaveLoadService.Instance.SaveProgress();
    }

    private void LoadItems()
    {
        Item[] allItems = Resources.LoadAll<Item>("Items");

        items.Clear();

        foreach (string itemId in saveFile.InventorySnapshot.ItemIds)
        {
            Item item = Array.Find(allItems, x => x.itemId == itemId);

            if (item != null)
                items.Add(item);
            else
                Debug.LogWarning("Item with itemId " + itemId + " not found in Resources.");
        }
    }
}
