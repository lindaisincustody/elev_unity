using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGamesManager : MonoBehaviour
{
    public void StartMazeGame()
    {
        Player.instance.SaveCurrentScenePosition();
        SceneManager.LoadScene(Constants.SceneNames.CoordinationGameScene);
    }

    public void StartNeutralityGame()
    {
        Player.instance.SaveCurrentScenePosition();
        SceneManager.LoadScene(Constants.SceneNames.NeutralityGameScene);
    }

    public void StartIntelligenceGame()
    {
        Player.instance.SaveCurrentScenePosition();
        SceneManager.LoadScene(Constants.SceneNames.IntelligenceGameScene);
    }

    public void StartStrengthGame()
    {
        Player.instance.SaveCurrentScenePosition();
        SceneManager.LoadScene(Constants.SceneNames.StrengthGameScene);
    }
}
