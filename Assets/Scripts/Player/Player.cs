using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject InteractableUI;
    [Header("Self-Referneces")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] InputManager inputManager;
    [SerializeField] private GameObject poemAvailable; // Add this line
    [SerializeField] private WordData[] wordsData;

    private DataManager dataManager;
    public PlayerData playerData;
    private TrainMovement trainMovement;


    public static Player instance { get; set; }
    public PlayerMovement GetPlayerMovement => playerMovement;
    public InputManager GetInputManager => inputManager;

    private Inventory _inventory = new Inventory();

    private void OnEnable()
    {
        ExperienceBar.instance.OnExperienceChange += HandleExperienceChange;
    }
    private void OnDisable()
    {
        ExperienceBar.instance.OnExperienceChange -= HandleExperienceChange;
    }

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Double Player or singleton problem");

        instance = this;

        dataManager = DataManager.Instance;

        trainMovement = GetComponent<TrainMovement>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "StationScene")
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
        }
        inputManager.OnPoem += OpenPoemBook;
        playerData = dataManager.GetPlayerData();
        SetUpPlayerData();

        dataManager.SavePosition(transform.position);
    }

    private void OnDestroy()
    {
        inputManager.OnPoem -= OpenPoemBook;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StationScene" && TrainMovement.hasArrived)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = true;
        }
    }

    private void SetUpPlayerData()
    {
        _inventory.SetUpData(playerData);
        _inventory.AddGold(playerData.gold);
        Vector2 lastPos = playerData.lastPos;
        if (lastPos != null)
            transform.position = lastPos;
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

    public int GetExprerience()
    {
        return ExperienceBar.instance.currentExperience;
    }

    private void HandleExperienceChange(int newExperience)
    {
        ExperienceBar.instance.currentExperience += newExperience;
        if (ExperienceBar.instance.currentExperience >= ExperienceBar.instance.maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerData.currentLevel++;
        dataManager.SavePlayerData(playerData);
        ExperienceBar.instance.currentExperience = 0;
        ExperienceBar.instance.maxExperience += 100;

        poemAvailable.SetActive(true);
    }

    private void OpenPoemBook()
    {
        if (playerData.currentLevel > playerData.poemsUsed)
        {
            int poemToOpen = playerData.currentLevel;
            if (playerData.currentLevel > wordsData.Length - 1)
            {
                poemToOpen = wordsData.Length - 1;
            }
            playerData.poemsUsed++;
            dataManager.SavePlayerData(playerData);
            PoemMenuController.instance.OpenPoemBook(wordsData[poemToOpen]);
            Debug.Log(playerData.currentLevel + " " + playerData.poemsUsed);
        }
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
        if (show == InteractableUI.activeSelf)
            return;

        SoundManager.PlaySound2D(SoundManager.Sound.QuietClick, 0.7f);
        InteractableUI.SetActive(show);
    }
}

public class PlayerData
{
    public string lastScene;
    public Vector2 lastPos;
    
    public int gold;

    public int currentExperience = 0;
    public int maxExperience = 300;
    public int currentLevel = 0;
    public int poemsUsed = 0;

    public float heroStrength;
    public float heroCoordination;
    public float heroIntelligence;
    public float heroNeutrality;

    public int StrengthLevel;
    public int CoordinationLevel;
    public int IntelligenceLevel;
    public int NeutralityLevel;
}

