using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGamesManager : MonoBehaviour
{
    [SerializeField] ActivateMazeGame mazeGameManager;
    public int StrengthLevel = 1;
    public int IntelligenceLevel = 1;
    public int CoordinationLevel = 1;
    public int NeutralityLevel = 1;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartMazeGame()
    {
        CoordinationLevel = PlayerPrefs.GetInt("CoordinationLevel", 1);
        
        mazeGameManager.LoadMazeMinigame(CoordinationLevel);
    }

    public void StartNeutralityGame()
    {
        SceneManager.LoadScene("NeutralityGame");
    }

    public void StartIntelligenceGame()
    {
        SceneManager.LoadScene("IntelligenceGame");
    }

    public void StartStrengthGame()
    {
        SceneManager.LoadScene("Strength");
    }
}
