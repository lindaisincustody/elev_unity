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

    public void LoadMazeMinigame()
    {
        mazeData.MazeHeight = (int)(attributes.heroCoordination / attributes.numberOfPoems * MaxMazeHeight);
        mazeData.MazeWidth = (int)(attributes.heroCoordination / attributes.numberOfPoems * MaxMazeWidth);
        if (mazeData.MazeHeight < 5)
            mazeData.MazeHeight = 5;
        if (mazeData.MazeWidth < 5)
            mazeData.MazeWidth = 5;
        SceneManager.LoadScene("Maze");
    }
}
