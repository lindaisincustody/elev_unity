using System.Collections.Generic;
using UnityEngine;

public class NavGrid
{
    public const float CellSize = 0.3333333f;

    private const float DiagonalCost = 1.4142136f;
    private const int NeighbourCount = 8;

    private static readonly int[] NeighbourX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    private static readonly int[] NeighbourY = { 0, 0, 1, -1, 1, -1, 1, -1 };

    private readonly Vector2 origin;
    private readonly int width;
    private readonly int height;
    private readonly bool[] blocked;

    private readonly float[] gScore;
    private readonly float[] fScore;
    private readonly int[] cameFrom;
    private readonly bool[] closed;
    private readonly bool[] opened;
    private readonly List<int> open = new List<int>();
    private static readonly Collider2D[] OverlapBuffer = new Collider2D[1];

    public NavGrid(Vector2 min, Vector2 max, Vector2 agentOffset, float agentRadius, LayerMask obstacles)
    {
        origin = min;
        width = Mathf.Max(1, Mathf.CeilToInt((max.x - min.x) / CellSize));
        height = Mathf.Max(1, Mathf.CeilToInt((max.y - min.y) / CellSize));

        int count = width * height;

        blocked = new bool[count];
        gScore = new float[count];
        fScore = new float[count];
        cameFrom = new int[count];
        closed = new bool[count];
        opened = new bool[count];

        ContactFilter2D filter = new ContactFilter2D { useTriggers = false, useLayerMask = true, layerMask = obstacles };

        for (int i = 0; i < count; i++)
            blocked[i] = Physics2D.OverlapCircle(CellCenter(i) + agentOffset, agentRadius, filter, OverlapBuffer) > 0;

    }

    public bool FindPath(Vector2 from, Vector2 to, List<Vector2> path)
    {
        path.Clear();

        int start = NearestFree(from);
        int goal = NearestFree(to);

        if (start < 0 || goal < 0)
            return false;

        for (int i = 0; i < blocked.Length; i++)
        {
            gScore[i] = float.MaxValue;
            cameFrom[i] = -1;
            closed[i] = false;
            opened[i] = false;
        }

        open.Clear();
        gScore[start] = 0f;
        fScore[start] = Heuristic(start, goal);
        open.Add(start);
        opened[start] = true;

        while (open.Count > 0)
        {
            int current = TakeLowest();

            if (current == goal)
            {
                Reconstruct(start, goal, path);

                if (path.Count == 0)
                    path.Add(CellCenter(goal));

                Smooth(path, from, to);
                return true;
            }

            closed[current] = true;

            int cx = current % width;
            int cy = current / width;

            for (int n = 0; n < NeighbourCount; n++)
            {
                int nx = cx + NeighbourX[n];
                int ny = cy + NeighbourY[n];

                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                    continue;

                int neighbour = ny * width + nx;

                if (blocked[neighbour] || closed[neighbour])
                    continue;

                if (n >= 4 && (blocked[cy * width + nx] || blocked[ny * width + cx]))
                    continue;

                float cost = gScore[current] + (n >= 4 ? DiagonalCost : 1f);

                if (cost >= gScore[neighbour])
                    continue;

                gScore[neighbour] = cost;
                fScore[neighbour] = cost + Heuristic(neighbour, goal);
                cameFrom[neighbour] = current;

                if (!opened[neighbour])
                {
                    open.Add(neighbour);
                    opened[neighbour] = true;
                }
            }
        }

        return false;
    }

    public bool TryGetNearestFree(Vector2 position, out Vector2 result)
    {
        int cell = NearestFree(position);

        result = cell < 0 ? position : CellCenter(cell);

        return cell >= 0;
    }

    public int Width => width;
    public int Height => height;

    public bool IsBlocked(int x, int y)
    {
        return blocked[y * width + x];
    }

    public Vector2 CellCenter(int x, int y)
    {
        return CellCenter(y * width + x);
    }

    public bool IsFree(Vector2 position)
    {
        int index = CellIndex(position);

        return index >= 0 && !blocked[index];
    }

    private int TakeLowest()
    {
        int best = 0;

        for (int i = 1; i < open.Count; i++)
        {
            if (fScore[open[i]] < fScore[open[best]])
                best = i;
        }

        int cell = open[best];
        open[best] = open[open.Count - 1];
        open.RemoveAt(open.Count - 1);
        opened[cell] = false;

        return cell;
    }

    private void Reconstruct(int start, int goal, List<Vector2> path)
    {
        for (int cell = goal; cell != start; cell = cameFrom[cell])
            path.Add(CellCenter(cell));

        path.Reverse();
    }

    private void Smooth(List<Vector2> path, Vector2 from, Vector2 to)
    {
        if (IsFree(to))
            path[path.Count - 1] = to;

        int i = 0;

        while (i + 2 < path.Count)
        {
            if (HasLineOfSight(path[i], path[i + 2]))
                path.RemoveAt(i + 1);
            else
                i++;
        }

        while (path.Count > 1 && HasLineOfSight(from, path[1]))
            path.RemoveAt(0);
    }

    private bool HasLineOfSight(Vector2 a, Vector2 b)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(a, b) / (CellSize * 0.5f));

        for (int i = 1; i < steps; i++)
        {
            if (!IsFree(Vector2.Lerp(a, b, i / (float)steps)))
                return false;
        }

        return true;
    }

    private float Heuristic(int cell, int goal)
    {
        float dx = Mathf.Abs(cell % width - goal % width);
        float dy = Mathf.Abs(cell / width - goal / width);

        return Mathf.Max(dx, dy) + (DiagonalCost - 1f) * Mathf.Min(dx, dy);
    }

    private int NearestFree(Vector2 position)
    {
        int index = CellIndex(position);

        if (index >= 0 && !blocked[index])
            return index;

        int cx = Mathf.Clamp(Mathf.FloorToInt((position.x - origin.x) / CellSize), 0, width - 1);
        int cy = Mathf.Clamp(Mathf.FloorToInt((position.y - origin.y) / CellSize), 0, height - 1);

        for (int radius = 1; radius < Mathf.Max(width, height); radius++)
        {
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                for (int x = cx - radius; x <= cx + radius; x++)
                {
                    if (x < 0 || y < 0 || x >= width || y >= height)
                        continue;

                    if (Mathf.Abs(x - cx) != radius && Mathf.Abs(y - cy) != radius)
                        continue;

                    if (!blocked[y * width + x])
                        return y * width + x;
                }
            }
        }

        return -1;
    }

    private int CellIndex(Vector2 position)
    {
        int x = Mathf.FloorToInt((position.x - origin.x) / CellSize);
        int y = Mathf.FloorToInt((position.y - origin.y) / CellSize);

        if (x < 0 || y < 0 || x >= width || y >= height)
            return -1;

        return y * width + x;
    }

    private Vector2 CellCenter(int index)
    {
        return new Vector2(
            origin.x + (index % width + 0.5f) * CellSize,
            origin.y + (index / width + 0.5f) * CellSize);
    }
}
