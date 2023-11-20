using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame : MonoBehaviour
{
    public SaveGameDictionary savedProgress = new SaveGameDictionary();
}

[Serializable]
public class Level
{
    public string levelId;
    public Vector2 playerPos;

    public Level(string levelId, Vector2 playerPos)
    {
        this.levelId = levelId;
        this.playerPos = playerPos;
    }
}

[Serializable]
public class SaveGameDictionary : UnitySerializedDictionary<string, Level> { }
