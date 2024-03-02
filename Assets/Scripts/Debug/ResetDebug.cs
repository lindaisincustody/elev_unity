using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetebug : MonoBehaviour
{
    public void ResetGamesLevels()
    {
        PlayerPrefs.SetInt(Constants.PlayerPrefs.StrengthLevel, 1);
        PlayerPrefs.SetInt(Constants.PlayerPrefs.CoordinationLevel, 1);
        PlayerPrefs.SetInt(Constants.PlayerPrefs.IntelligenceLevel, 1);
        PlayerPrefs.SetInt(Constants.PlayerPrefs.NeutralityLevel, 1);
        Debug.Log("))");
    }
}
