using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCursorManager : MonoBehaviour
{
    [SerializeField] UIElementsHolder uIElements;

    private GeneralSaveFile saveFile;
    private SceneController sceneController;

    private bool submit = false;

    private void Start()
    {
        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
        sceneController = UIManager.Instance.Get<SceneController>();
    }

    public void StartGame()
    {
        if (submit)
            return;

        submit = true;
        string lastScene = saveFile.SceneSnapshot.SceneName;
        if (string.IsNullOrEmpty(lastScene))
            StartCoroutine(sceneController.LoadScene(Constants.SceneNames.TrainStation));
        else
            StartCoroutine(sceneController.LoadScene(lastScene));
    }

    public void OpenControls()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
