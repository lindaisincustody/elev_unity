using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCursorManager : MonoBehaviour
{
    [SerializeField] CursorController cursor;
    [SerializeField] UIElementsHolder uIElements;
    [SerializeField] UIElementsHolder controlsUIElements;

    [SerializeField] GameObject ControlsPanel;

    private bool isControlsPanelOpen = false;

    private GeneralSaveFile saveFile;
    private SceneController sceneController;

    private bool submit = false;

    private void Start()
    {
        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
        sceneController = UIManager.Instance.Get<SceneController>();
        cursor.ActivateCursor(uIElements.cursorElements, null);
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
        if (!isControlsPanelOpen)
        {
            ControlsPanel.SetActive(true);
            cursor.ActivateCursor(controlsUIElements.cursorElements, null);
            isControlsPanelOpen = true;
        }
        else
        {
            ControlsPanel.SetActive(false);
            cursor.ActivateCursor(uIElements.cursorElements, null);
            isControlsPanelOpen = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
