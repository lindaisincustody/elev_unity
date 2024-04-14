using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject InteractableUI;
    [Header("Self-Referneces")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] InputManager inputManager;

    private DataManager dataManager;
    private PlayerData playerData;

    public static Player instance { get; set; }
    public PlayerMovement GetPlayerMovement => playerMovement;
    public InputManager GetInputManager => inputManager;

    private Inventory _inventory = new Inventory();

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Double Player or singleton problem");

        instance = this;

        dataManager = DataManager.Instance;
    }

    private void Start()
    {
        playerData = dataManager.GetPlayerData();
        SetUpPlayerData();
    }

    private void SetUpPlayerData()
    {
        _inventory.SetUpData(playerData);
        _inventory.AddGold(playerData.gold);
        //Vector2 lastPos = playerData.lastPos;
        //if (lastPos != null)
        //    transform.position = lastPos;
    }

    public void AddGold(int goldAmount)
    {
        _inventory.AddGold(goldAmount);

        playerData.gold = _inventory.GetGold();
        dataManager.SavePlayerData(playerData);
    }

    public int GetGold()
    {
        return _inventory.GetGold();
    }

    public void AddGoldMultiplier(Attribute attribute, float multiplier)
    {
        _inventory.AddGoldMultiplier(attribute, multiplier);

        playerData.heroStrength = _inventory.GetGoldMultiplier(Attribute.Strength);
        playerData.heroCoordination = _inventory.GetGoldMultiplier(Attribute.Coordination);
        playerData.heroIntelligence = _inventory.GetGoldMultiplier(Attribute.Intelligence);
        playerData.heroNeutrality = _inventory.GetGoldMultiplier(Attribute.Neutrality);
        dataManager.SavePlayerData(playerData);
    }

    public float GetGoldMultiplier(Attribute attribute)
    {
        return _inventory.GetGoldMultiplier(attribute);
    }

    public void SetMovement(bool canMove)
    {
        playerMovement.SetMovement(canMove);
    }

    public void ShowInteractUI(bool show)
    {
        InteractableUI.SetActive(show);
    }
}

public class PlayerData
{
    public string lastScene;
    public Vector2 lastPos;
    
    public int gold;

    public float heroStrength;
    public float heroCoordination;
    public float heroIntelligence;
    public float heroNeutrality;

    public int StrengthLevel;
    public int CoordinationLevel;
    public int IntelligenceLevel;
    public int NeutralityLevel;
}

