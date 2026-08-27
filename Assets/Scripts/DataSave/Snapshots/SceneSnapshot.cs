using System;
using UnityEngine;

[Serializable]
public class SceneSnapshot
{
    public string SceneName;
    public Vector3 PlayerPosition;
    public bool HasPlayerPosition;

    public SceneSnapshot()
    {
    }

    public SceneSnapshot(string sceneName, Vector3 playerPosition)
    {
        SceneName = sceneName;
        PlayerPosition = playerPosition;
        HasPlayerPosition = true;
    }
}
