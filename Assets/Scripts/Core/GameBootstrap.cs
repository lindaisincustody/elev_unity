using UnityEngine;

public static class GameBootstrap
{
    public const string PrefabPath = "Bootstrap";

    private static GameObject root;

    public static bool IsLoaded => root != null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (root != null) return;

        GameObject prefab = Resources.Load<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[GameBootstrap] No prefab found at Resources/{PrefabPath}. Global managers will not be created.");
            return;
        }

        root = Object.Instantiate(prefab);
        root.name = prefab.name;
        Object.DontDestroyOnLoad(root);
    }
}
