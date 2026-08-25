using System;
using UnityEngine;

[Serializable]
public class SceneSnapshot
{
    public string SceneName;
    public Vector3 PlayerPosition;

    public SceneSnapshot()
    {
    }

    public SceneSnapshot(string sceneName, Vector3 playerPosition)
    {
        SceneName = sceneName;
        PlayerPosition = playerPosition;
    }
}
