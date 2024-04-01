using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberDisplayer : MonoBehaviour
{
    [SerializeField] GameObject[] lines;

    private int number = 0;
     
    public void ShowNumber(int number)
    {
        switch (number)
        {
            case 0:
                ShowNumberZero();
                break;
            case 1:
                ShowNumberOne();
                break;
            case 2:
                ShowNumberTwo();
                break;
            case 3:
                ShowNumberThree();
                break;
            case 4:
                ShowNumberFour();
                break;
            case 5:
                ShowNumberFive();
                break;
            case 6:
                ShowNumberSix();
                break;
            case 7:
                ShowNumberSeven();
                break;
            case 8:
                ShowNumberEight();
                break;
            case 9:
                ShowNumberNine();
                break;
            default:
                Debug.LogError("Invalid number: " + number);
                break;
        }
    }
    private void ShowNumberOne()
    {
        lines[0].SetActive(false);
        lines[1].SetActive(false);
        lines[2].SetActive(true);
        lines[3].SetActive(false);
        lines[4].SetActive(false);
        lines[5].SetActive(true);
        lines[6].SetActive(false);
    }

    private void ShowNumberTwo()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(false);
        lines[2].SetActive(true);
        lines[3].SetActive(true);
        lines[4].SetActive(true);
        lines[5].SetActive(false);
        lines[6].SetActive(true);
    }

    private void ShowNumberThree()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(false);
        lines[2].SetActive(true);
        lines[3].SetActive(true);
        lines[4].SetActive(false);
        lines[5].SetActive(true);
        lines[6].SetActive(true);
    }

    private void ShowNumberFour()
    {
        lines[0].SetActive(false);
        lines[1].SetActive(true);
        lines[2].SetActive(true);
        lines[3].SetActive(true);
        lines[4].SetActive(false);
        lines[5].SetActive(true);
        lines[6].SetActive(false);
    }

    private void ShowNumberFive()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(true);
        lines[2].SetActive(false);
        lines[3].SetActive(true);
        lines[4].SetActive(false);
        lines[5].SetActive(true);
        lines[6].SetActive(true);
    }

    private void ShowNumberSix()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(true);
        lines[2].SetActive(false);
        lines[3].SetActive(true);
        lines[4].SetActive(true);
        lines[5].SetActive(true);
        lines[6].SetActive(true);
    }

    private void ShowNumberSeven()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(false);
        lines[2].SetActive(true);
        lines[3].SetActive(false);
        lines[4].SetActive(false);
        lines[5].SetActive(true);
        lines[6].SetActive(false);
    }

    private void ShowNumberEight()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(true);
        lines[2].SetActive(true);
        lines[3].SetActive(true);
        lines[4].SetActive(true);
        lines[5].SetActive(true);
        lines[6].SetActive(true);
    }

    private void ShowNumberNine()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(true);
        lines[2].SetActive(true);
        lines[3].SetActive(true);
        lines[4].SetActive(false);
        lines[5].SetActive(true);
        lines[6].SetActive(true);
    }

    private void ShowNumberZero()
    {
        lines[0].SetActive(true);
        lines[1].SetActive(true);
        lines[2].SetActive(true);
        lines[3].SetActive(false);
        lines[4].SetActive(true);
        lines[5].SetActive(true);
        lines[6].SetActive(true);
    }

}
