using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateMazeGame : MonoBehaviour
{
    [SerializeField] private Attributes attributes;
    [SerializeField] private MazeDataSO mazeData;
    [Range(4, 30)]
    public int MaxMazeHeight;
    [Range(4, 30)]
    public int MaxMazeWidth;

    public DataManager dataManager;

    public void LoadMazeMinigame(int level)
    {
        mazeData.MazeHeight = level + 5;
        mazeData.MazeWidth = level + 5;


        dataManager.SavePlayerPosition(() =>
        {
            LoadNewScene("Maze");
        });
        // Call this after saving
        LoadNewScene("Maze");
    }

    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
