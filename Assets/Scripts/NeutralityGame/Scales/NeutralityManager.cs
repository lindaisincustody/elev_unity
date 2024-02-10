using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralityManager : MonoBehaviour
{
    private int scoreToWin = 4;
    private int scoreLossdDifference;

    public Action gameStart;

    private void Start()
    {
        scoreLossdDifference = (int)(3 * 0.6f);
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        gameStart?.Invoke();
    }

    public void Win()
    {
        Debug.Log("Neutrality Game Won!");
    }

    public void Lose()
    {
        Debug.Log("Neutrality Game Lost!");
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
