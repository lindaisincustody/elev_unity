using UnityEngine;

public class PlayerPoems : Component
{
    [SerializeField] private GameObject poemAvailable;
    [SerializeField] private WordData[] wordsData;

    private GeneralSaveFile saveFile;

    private void Awake()
    {
        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
    }

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

    private void Start()
    {
        InputManager.Instance.OnPoem += OpenPoemBook;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnPoem -= OpenPoemBook;
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
}
