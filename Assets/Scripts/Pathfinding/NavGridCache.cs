using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NavGridCache
{
    private static readonly Dictionary<string, NavGrid> grids = new Dictionary<string, NavGrid>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += (scene, mode) => grids.Clear();
    }

    public static NavGrid Get(Vector2 min, Vector2 max, Vector2 agentOffset, float agentRadius, LayerMask obstacles)
    {
        string key = $"{min.x:F2},{min.y:F2},{max.x:F2},{max.y:F2},{agentOffset.x:F2},{agentOffset.y:F2},{agentRadius:F2},{obstacles.value}";

        if (!grids.TryGetValue(key, out NavGrid grid))
        {
            grid = new NavGrid(min, max, agentOffset, agentRadius, obstacles);
            grids[key] = grid;
        }

        return grid;
    }
}
