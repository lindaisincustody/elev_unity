using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NeutralityManager : MonoBehaviour
{
    private int scoreToWin = 4;
    private int scoreLossdDifference;

    public Action gameStart;

    private void Awake()
    {
        //scoreToWin =  PlayerPrefs.GetInt(Constants.PlayerPrefs.NeutralityLevel) + 2;
    }

    private void Start()
    {
        scoreLossdDifference = (int)(3 * 0.6f);
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Win();
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        gameStart?.Invoke();
    }

    public void Win()
    {
        Debug.Log("You Won");
        scoreToWin++;
       // int level = PlayerPrefs.GetInt(Constants.PlayerPrefs.NeutralityLevel);
       // level++;
        //PlayerPrefs.SetInt(Constants.PlayerPrefs.NeutralityLevel, level);
        SceneManager.LoadScene(Constants.SceneNames.MainScene);
    }

    public void Lose()
    {
        Debug.Log("You Lost");
        SceneManager.LoadScene(Constants.SceneNames.MainScene);
    }

    public int GetScoreToWin()
    {
        return scoreToWin;
    }

    public void SetScoreToWIn(int newScore)
    {
        scoreToWin = newScore;
    }

    public int GetDifferenceToLose()
    {
        return scoreLossdDifference;
    }
}
