using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SavingWrapper : MonoBehaviour
{
    private static SavingWrapper instance;

    private string inventorySaveFileName = "inventory.json";
    private string playerDataSaveFileName = "playerData.json";

    public static SavingWrapper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject savingWrapperObject = new GameObject("SavingWrapper");
                instance = savingWrapperObject.AddComponent<SavingWrapper>();
                DontDestroyOnLoad(savingWrapperObject);
            }
            return instance;
        }
    }

    public void SaveInventory(InventoryData inventoryData)
    {
        List<string> itemIdList = new List<string>();
        foreach (var item in inventoryData.items)
        {
            itemIdList.Add(item.itemId);
        }

        // Serialize the itemIdList using Newtonsoft.Json
        string json = JsonConvert.SerializeObject(itemIdList);

        // Write the JSON string to a file
        File.WriteAllText(Path.Combine(Application.persistentDataPath, inventorySaveFileName), json);
    }


    public InventoryData LoadInventory()
    {
        ShopItem[] shopItems = Resources.LoadAll<ShopItem>("Items");

        string path = Path.Combine(Application.persistentDataPath, inventorySaveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            List<string> itemIdList = JsonConvert.DeserializeObject<List<string>>(json); // Deserialize using JsonConvert

            InventoryData inventoryData = new InventoryData();
            foreach (var itemId in itemIdList)
            {
                // Find the ShopItem with the corresponding itemId
                ShopItem item = Array.Find(shopItems, x => x.itemId == itemId);
                if (item != null)
                {
                    inventoryData.items.Add(item);
                }
                else
                {
                    Debug.LogWarning("ShopItem with itemId " + itemId + " not found in Resources.");
                }
            }

            return inventoryData;
        }

        return new InventoryData();
    }

    public void SavePlayerData(PlayerData playerData)
    {
        // Add more data to save as needed
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, playerDataSaveFileName), json);
    }

    public PlayerData LoadPlayerData()
    {
        string path = Path.Combine(Application.persistentDataPath, playerDataSaveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }

        return new PlayerData {
            heroStrength = 1,
            heroNeutrality = 1,
            heroIntelligence = 1,
            heroCoordination = 1,
            
            StrengthLevel = 1,
            NeutralityLevel = 1,
            IntelligenceLevel = 1,
            CoordinationLevel = 1,

            gold = 0
        };
    }

    public void DeleteAllData()
    {
        string path = Path.Combine(Application.persistentDataPath, inventorySaveFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        path = Path.Combine(Application.persistentDataPath, playerDataSaveFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
