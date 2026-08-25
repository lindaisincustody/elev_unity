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
    [field: SerializeField] public Animator Animator { get; private set; }
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] private GameObject poemAvailable;
    [SerializeField] private WordData[] wordsData;
    [SerializeField] private Collider2D bodyCollider;

    public float SpecialSymbolChance { get; set; }

    private GeneralSaveFile saveFile;

    public static Player instance { get; private set; }

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
        if (instance != null)
            Debug.LogError("Double Player or singleton problem");

        instance = this;

        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
    }

    private void Start()
    {
        InputManager.Instance.OnPoem += OpenPoemBook;
        _inventory.AddGold(saveFile.PlayerSnapshot.Gold);
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnPoem -= OpenPoemBook;
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

        saveFile.PlayerSnapshot.Gold = _inventory.GetGold();
        SaveLoadService.Instance.SaveProgress();
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
        saveFile.PlayerSnapshot.CurrentLevel++;
        SaveLoadService.Instance.SaveProgress();
        ExperienceBar.instance.currentExperience = 0;
        ExperienceBar.instance.maxExperience += 100;

        poemAvailable.SetActive(true);
    }

    private void OpenPoemBook()
    {
        poemAvailable.SetActive(false);

        PlayerSnapshot player = saveFile.PlayerSnapshot;

        if (player.CurrentLevel > player.PoemsUsed)
        {
            int poemToOpen = player.CurrentLevel;
            if (player.CurrentLevel > wordsData.Length - 1)
            {
                poemToOpen = wordsData.Length - 1;
            }
            player.PoemsUsed++;
            SaveLoadService.Instance.SaveProgress();
            UIManager.Instance.Get<PoemMenuController>().OpenPoemBook(wordsData[poemToOpen]);
        }
    }

    public void AddGoldMultiplier(Attribute attribute, float multiplier)
    {
        _inventory.AddGoldMultiplier(attribute, multiplier);

        SaveLoadService.Instance.SaveProgress();
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
        saveFile.SceneSnapshot.SceneName = SceneManager.GetActiveScene().name;
        saveFile.SceneSnapshot.PlayerPosition = transform.position;
        SaveLoadService.Instance.SaveProgress();
    }

    public Vector3? GetSavedScenePosition()
    {
        SceneSnapshot scene = saveFile.SceneSnapshot;

        if (scene.SceneName == SceneManager.GetActiveScene().name)
            return scene.PlayerPosition;

        return null;
    }
}
