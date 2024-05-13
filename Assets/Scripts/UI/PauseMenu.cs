using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject background;
    [SerializeField] GameObject content;
    [SerializeField] CursorController cursor;
    [SerializeField] UIElementsHolder elements;

    private InputManager playerInput;


    private void Start()
    {
        if (Player.instance != null)
            playerInput = Player.instance.GetInputManager;
        playerInput = FindObjectOfType<InputManager>();
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
        StartCoroutine(SceneController.instance.LoadScene(Constants.SceneNames.MainMenu));
    }

    private void OnDestroy()
    {
        playerInput.OnCancel -= PauseGame;
    }
}
