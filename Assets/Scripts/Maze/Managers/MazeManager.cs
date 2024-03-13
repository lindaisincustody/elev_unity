using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MazeManager : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] MazeScreen loseScreen;
    [SerializeField] MazeWinScreen winScreen;
    [SerializeField] MazePlayerMovement playerMovement;
    [SerializeField] CircleCollider2D playerCollider;

    private void Start()
    {
        StartMaze();
    }

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

    private void StartMaze()
    {
        int level = PlayerPrefs.GetInt(Constants.PlayerPrefs.CoordinationLevel);
        mazeGenerator.StartGeneratingMaze(level + 5, level + 5);
    }
}
