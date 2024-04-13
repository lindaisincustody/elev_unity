using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MazeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] MazeScreen loseScreen;
    [SerializeField] MazeWinScreen winScreen;
    [SerializeField] MazePlayerMovement playerMovement;
    [SerializeField] CircleCollider2D playerCollider;

    [Header("Item references")]
    [SerializeField] PathShowersSpawner pathShowersSpawner;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Win();
    }

    public void Win()
    {
        var level = PlayerPrefs.GetInt(Constants.PlayerPrefs.CoordinationLevel, 1);
        level++;
        PlayerPrefs.SetInt(Constants.PlayerPrefs.CoordinationLevel, level);
        playerCollider.enabled = false;
        playerMovement.StopPlayer();
        winScreen.ShowEndScreen();
    }

    public void Lose()
    {
        playerCollider.enabled = false;
        playerMovement.StopPlayer();
        loseScreen.ShowEndScreen();
    }

    public void StartMaze()
    {
        int level = PlayerPrefs.GetInt(Constants.PlayerPrefs.CoordinationLevel);
        mazeGenerator.StartGeneratingMaze(level + 5, level + 5);
    }

    // Item methods
    public void IncreaseLuck(float luckIncrease)
    {
        Debug.Log("Luck increase byL " + luckIncrease);
    }

    public void IncreasePathShowerAmount(int additionalAmount)
    {
        pathShowersSpawner.IncreasePathShowerCount(additionalAmount);
    }
}
