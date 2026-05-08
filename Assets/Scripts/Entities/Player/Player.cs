using System.Collections;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [SerializeField] GameObject InteractableUI;
    [Header("Self-Referneces")]
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] InputManager inputManager;
    [SerializeField] private GameObject poemAvailable;
    [SerializeField] private WordData[] wordsData;
    [SerializeField] private Collider2D bodyCollider;

    public float SpecialSymbolChance { get; set; }

    private DataManager dataManager;
    public PlayerData playerData;

    public static Player instance { get; set; }
    public InputManager GetInputManager => inputManager;

    public bool InDangerZone;
    public bool InSafeZone;

    public System.Action<Collider2D> OnGhost;


    private Inventory _inventory = new Inventory();

    private void OnEnable()
    {
        if (ExperienceBar.instance != null)
            ExperienceBar.instance.OnExperienceChange += HandleExperienceChange;
    }
    private void OnDisable()
    {
        if (ExperienceBar.instance != null)
            ExperienceBar.instance.OnExperienceChange -= HandleExperienceChange;
    }

    private void Awake()
    {
        // In multiplayer two Player instances exist on this device (local + remote proxy).
        // Only the first one to Awake sets the singleton; NetworkPlayerSync.OnNetworkSpawn()
        // then re-assigns it to whichever object IsOwner == true (the local player).
        // This prevents the remote proxy's Awake from stomping the host's own reference.
        if (instance == null)
            instance = this;

        dataManager = DataManager.Instance;
    }

    private void Start()
    {
        // inputManager is an Inspector-assigned reference that is null on the spawned
        // prefab clone (prefabs can't hold scene-object refs). Guard every access.
        if (inputManager == null)
            inputManager = GetComponent<InputManager>();

        if (inputManager != null)
            inputManager.OnPoem += OpenPoemBook;
        else
            Debug.LogWarning("[Player] inputManager is null — OnPoem will not fire. " +
                             "Assign InputManager in the Player prefab Inspector.");

        playerData = dataManager.GetPlayerData();
        SetUpPlayerData();
    }

    private void OnDestroy()
    {
        if (inputManager != null)
            inputManager.OnPoem -= OpenPoemBook;
    }

    private void SetUpPlayerData()
    {
        _inventory.AddGold(playerData.gold);

        // Only restore the saved position for the scene-placed (P1) player.
        // The network-spawned clone (P2) is positioned by PlayerSpawner at its
        // spawn point; teleporting it here would place it at P1's last position.
        var netObj = GetComponent<Unity.Netcode.NetworkObject>();
        bool isNetworkSpawned = netObj != null && netObj.IsSpawned;
        bool isLocalOwner     = netObj != null && netObj.IsOwner;

        // Only apply saved position to the local P1 (host, not a spawned prefab).
        // P1 is the scene-placed player: it has no NetworkObject yet when Start()
        // first fires, OR it is the owner and has a saved position.
        if (!isNetworkSpawned || (isLocalOwner && isNetworkSpawned))
        {
            Vector3? savedPosition = GetSavedScenePosition();
            if (savedPosition.HasValue)
            {
                transform.position = savedPosition.Value;
            }
        }
    }

    public void ToggleGhostForm()
    {
        if (!bodyCollider.enabled)
        {
            bodyCollider.enabled = true;
            return;
        }
        bodyCollider.enabled = false;
        OnGhost?.Invoke(bodyCollider);
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
        poemAvailable.SetActive(false);
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
        }
    }

    public void AddGoldMultiplier(Attribute attribute, float multiplier)
    {
        _inventory.AddGoldMultiplier(attribute, multiplier);

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

    public void ShowPlayer(bool show)
    {
        spriteRenderer.enabled = show;
    }

    public void SaveCurrentScenePosition()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        var scenePosition = playerData.scenePositions.Find(sp => sp.sceneName == currentScene);
        if (scenePosition != null)
        {
            scenePosition.lastPos = transform.position;
        }
        else
        {
            playerData.scenePositions.Add(new ScenePosition { sceneName = currentScene, lastPos = transform.position });
        }
        dataManager.SavePlayerData(playerData);
    }

    // Method to get the saved position for the current scene
    public Vector3? GetSavedScenePosition()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        var scenePosition = playerData.scenePositions.Find(sp => sp.sceneName == currentScene);
        if (scenePosition != null)
        {
            return scenePosition.lastPos;
        }
        return null;
    }
}

public class PlayerData
{
    public string lastScene;
    public string lastFightID;

    public int gold;

    public int currentExperience = 0;
    public int maxExperience = 300;
    public int currentLevel = 0;
    public int poemsUsed = 0;

    public List<ScenePosition> scenePositions = new List<ScenePosition>();
    public bool tutorialComplete = false;
    public float pillTimeLeft = 0;
}

[System.Serializable]
public class ScenePosition
{
    public string sceneName;
    public Vector3 lastPos;
}