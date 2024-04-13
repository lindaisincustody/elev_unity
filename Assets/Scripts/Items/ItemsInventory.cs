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
    private string saveFileName = "inventory.json";

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
        LoadInventory();
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(ShopItem newItem)
    {
        inventoryData.items.Add(newItem);
        SaveInventory();
    }

    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(inventoryData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), json);
    }
   

    public void LoadInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            inventoryData = JsonUtility.FromJson<InventoryData>(json);
        }
    }

    public void DeleteItems()
    {
        inventoryData.items.Clear();
        SaveInventory();
    }

    public List<ShopItem> GetAllItems()
    {
        return inventoryData.items;
    }
}
