using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PathGuide : CoreService
{
    public static PathGuide Instance { get; private set; }

    [SerializeField] private PathTrail trailPrefab;
    [SerializeField] private LayerMask obstacles = 1;
    [SerializeField] private float agentRadius = 0.3f;
    [SerializeField] private float searchMargin = 8f;
    [SerializeField] private int searchAttempts = 3;
    [SerializeField] private float chunkSize = 8f;

    private readonly List<Vector2> scratch = new List<Vector2>();

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public PathTrail Show(Vector2 from, Vector2 to)
    {
        List<Vector2> points = new List<Vector2> { from };

        if (TryFindPath(from, to, scratch, out _))
            points.AddRange(scratch);

        PathTrail trail = Instantiate(trailPrefab);
        trail.Show(points, to, Player.instance.transform);

        return trail;
    }

    public bool TryFindPath(Vector2 from, Vector2 to, List<Vector2> path, out NavGrid grid)
    {
        float margin = searchMargin;

        for (int attempt = 0; attempt < searchAttempts; attempt++, margin *= 2f)
        {
            Vector2 min = Snap(Vector2.Min(from, to) - Vector2.one * margin, Mathf.Floor);
            Vector2 max = Snap(Vector2.Max(from, to) + Vector2.one * margin, Mathf.Ceil);

            grid = NavGridCache.Get(min, max, Vector2.zero, agentRadius, obstacles);

            if (grid.FindPath(from, to, path))
                return true;

            if (!grid.ReachedEdge)
                break;

            if (!grid.FindPath(to, from, path) && !grid.ReachedEdge)
                break;
        }

        grid = null;
        return false;
    }

    private Vector2 Snap(Vector2 value, Func<float, float> round)
    {
        return new Vector2(round(value.x / chunkSize) * chunkSize, round(value.y / chunkSize) * chunkSize);
    }
}
