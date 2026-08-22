using UnityEngine;

public static class GameBootstrap
{
    public const string PrefabPath = "Bootstrap";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        GameObject root = Object.Instantiate(Resources.Load<GameObject>(PrefabPath));
        root.name = PrefabPath;
        Object.DontDestroyOnLoad(root);
    }
}
