using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    private SavingWrapper savingWrapper;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("DataManager");
                    instance = singletonObject.AddComponent<DataManager>();
                }
            }
            return instance;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savingWrapper = SavingWrapper.Instance;
        _inventory = savingWrapper.LoadInventory();
        playerData = savingWrapper.LoadPlayerData();
    }

    private PlayerData playerData = new PlayerData();
    private InventoryData _inventory = new InventoryData();

    public void SavePlayerData(PlayerData playerData)
    {
        savingWrapper.SavePlayerData(playerData);
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }


    public InventoryData GetInventoryData()
    {
        return _inventory;
    }

    public void SavePillTime()
    {
        playerData.pillTimeLeft = InventoryUI.Instance.savedDuration;
        SavePlayerData(playerData);
    }

    public void CompleteTutorial()
    {
        playerData.tutorialComplete = true;
        SavePlayerData(playerData);
    }

    public string GetLastScene()
    {
        return playerData.lastScene;
    }

    public void AddGold(int goldAmount)
    {
        playerData.gold += goldAmount;
        SavePlayerData(playerData);
    }

    public void SaveScene(string scene)
    {
        playerData.lastScene = scene;
        SavePlayerData(playerData);
    }

    public int GetLevel(Attribute attribute)
    {
        switch (attribute)
        {
            case Attribute.Strength:
                {
                    return playerData.StrengthLevel;
                }
            case Attribute.Intelligence:
                {
                    return playerData.IntelligenceLevel;
                }
            case Attribute.Coordination:
                {
                    return playerData.CoordinationLevel;
                }
            case Attribute.Neutrality:
                {
                    return playerData.NeutralityLevel;
                }
        }

        return -1;
    }

    public void AddLevel(Attribute attribute)
    {
        switch (attribute)
        {
            case Attribute.Strength:
                {
                    playerData.StrengthLevel++;
                    break;
                }
            case Attribute.Intelligence:
                {
                    playerData.IntelligenceLevel++;
                    break;
                }
            case Attribute.Coordination:
                {
                    playerData.CoordinationLevel++;
                    break;
                }
            case Attribute.Neutrality:
                {
                    playerData.NeutralityLevel++;
                    break;
                }
        }

        SavePlayerData(playerData);
    }
}
