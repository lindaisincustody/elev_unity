using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGamesManager : MonoBehaviour
{
    private DataManager dataManager;

    private void Awake()
    {
        dataManager = DataManager.Instance;
    }

    public void StartMazeGame()
    {
        dataManager.SavePosition(Player.instance.transform.position);
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
