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
        scoreToWin =  PlayerPrefs.GetInt("NeutralityLevel") + 2;
    }

    private void Start()
    {
        scoreLossdDifference = (int)(3 * 0.6f);
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Win();
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        gameStart?.Invoke();
    }

    public void Win()
    {
        scoreToWin++;
        int level = PlayerPrefs.GetInt("NeutralityLevel");
        level++;
        PlayerPrefs.SetInt("NeutralityLevel", level);
        SceneManager.LoadScene("SampleScene");
    }

    public void Lose()
    {
        SceneManager.LoadScene("SampleScene");
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
