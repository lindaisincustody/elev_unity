using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGamesManager : MonoBehaviour
{
    public int StrengthLevel = 1;
    public int IntelligenceLevel = 1;
    public int CoordinationLevel = 1;
    public int NeutralityLevel = 1;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        StrengthLevel = PlayerPrefs.GetInt(Constants.StrengthGameName, 1);
        IntelligenceLevel = PlayerPrefs.GetInt(Constants.IntelligenceGameName, 1);
        CoordinationLevel = PlayerPrefs.GetInt(Constants.CoordinationGameName, 1);
        NeutralityLevel = PlayerPrefs.GetInt(Constants.NeutralityGameName, 1);
    }

    public void StartMazeGame()
    {
        SceneManager.LoadScene(Constants.SceneNames.CoordinationGameScene);
    }

    public void StartNeutralityGame()
    {
        SceneManager.LoadScene(Constants.SceneNames.NeutralityGameScene);
    }

    public void StartIntelligenceGame()
    {
        SceneManager.LoadScene(Constants.SceneNames.IntelligenceGameScene);
    }

    public void StartStrengthGame()
    {
        SceneManager.LoadScene(Constants.SceneNames.StrengthGameScene);
    }
}
