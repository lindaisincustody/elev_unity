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

    private DataManager dataManager;

    private void Start()
    {
        dataManager = DataManager.Instance;
        cursor.ActivateCursor(uIElements.cursorElements, null);
    }

    public void StartGame()
    {
        string lastScene = dataManager.GetLastScene();
        if (string.IsNullOrEmpty(lastScene))
            StartCoroutine(SceneController.instance.LoadScene(Constants.SceneNames.MainScene));
        else
            StartCoroutine(SceneController.instance.LoadScene(lastScene));
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
