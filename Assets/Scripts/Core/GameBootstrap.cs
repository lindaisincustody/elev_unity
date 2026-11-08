using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameBootstrap
{
    public const string PrefabPath = "Bootstrap";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (SceneManager.GetActiveScene().name == Constants.SceneNames.SymbolTest)
            return;

        GameObject root = Object.Instantiate(Resources.Load<GameObject>(PrefabPath));
        root.name = PrefabPath;
        Object.DontDestroyOnLoad(root);
    }
}
