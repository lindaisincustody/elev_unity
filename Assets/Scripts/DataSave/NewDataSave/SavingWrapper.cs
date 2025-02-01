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
    private string abilitiesSaveFileName = "abilities.json";

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

    public void SavePlayerAbilities(List<Ability> abilities)
    {
        PlayerAbilitiesData data = new PlayerAbilitiesData();
        foreach (var ability in abilities)
        {
            data.abilityIds.Add(ability.abilityId);
        }

        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, abilitiesSaveFileName), json);
    }

    public List<Ability> LoadPlayerAbilities()
    {
        Ability[] allAbilities = Resources.LoadAll<Ability>("Abilities");
        string path = Path.Combine(Application.persistentDataPath, abilitiesSaveFileName);
        List<Ability> loadedAbilities = new List<Ability>();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerAbilitiesData data = JsonConvert.DeserializeObject<PlayerAbilitiesData>(json);

            foreach (var id in data.abilityIds)
            {
                Ability ability = System.Array.Find(allAbilities, a => a.abilityId == id);
                if (ability != null)
                {
                    loadedAbilities.Add(ability);
                }
                else
                {
                    Debug.LogWarning("Ability with ID " + id + " not found in Resources.");
                }
            }
        }

        return loadedAbilities;
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
        Item[] shopItems = Resources.LoadAll<Item>("Items");

        string path = Path.Combine(Application.persistentDataPath, inventorySaveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            List<string> itemIdList = JsonConvert.DeserializeObject<List<string>>(json); // Deserialize using JsonConvert

            InventoryData inventoryData = new InventoryData();
            foreach (var itemId in itemIdList)
            {
                // Find the ShopItem with the corresponding itemId
                Item item = Array.Find(shopItems, x => x.itemId == itemId);
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
