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

    public void LoadMazeMinigame()
    {
        mazeData.MazeHeight = (int)(attributes.heroCoordination / attributes.numberOfPoems * MaxMazeHeight);
        mazeData.MazeWidth = (int)(attributes.heroCoordination / attributes.numberOfPoems * MaxMazeWidth);
        Debug.Log(attributes.heroCoordination);
        Debug.Log(attributes.numberOfPoems);
        Debug.Log(MaxMazeHeight);
        if (mazeData.MazeHeight < 5)
            mazeData.MazeHeight = 5;
        if (mazeData.MazeWidth < 5)
            mazeData.MazeWidth = 5;

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
