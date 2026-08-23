using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject background;
    [SerializeField] GameObject content;
    [SerializeField] CursorController cursor;
    [SerializeField] UIElementsHolder elements;

    private SceneController sceneController;
    private InputManager playerInput;


    private void Start()
    {
        playerInput = Player.instance.GetInputManager;
        sceneController = UIManager.Instance.Get<SceneController>();

        playerInput.OnCancel += PauseGame;
    }

    private void PauseGame()
    {
        background.SetActive(true);
        content.SetActive(true);
        cursor.ActivateCursor(elements.cursorElements, null);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        background.SetActive(false);
        content.SetActive(false);
        cursor.DeactivateCursor();
        Time.timeScale = 1f;
    }

    public void OpenMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(sceneController.LoadScene(Constants.SceneNames.MainMenu));
    }

    private void OnDestroy()
    {
        playerInput.OnCancel -= PauseGame;
    }
}
