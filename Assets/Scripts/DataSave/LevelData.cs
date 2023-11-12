using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/LevelData")]
public class LevelData : ScriptableObject
{
    public GameObject[] poemTriggers;

    public string levelName = "";
    public string levelId = "";

    public string lastLevelId = "";
    public string nextLevelId = "";

    public bool isLoaded = false;

    public Level loadedSaveData;
}
