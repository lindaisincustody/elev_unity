using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TemplateSelection
{
    public const int PerSymbol = 5;

    public static List<int> Pick(IReadOnlyList<Vector2[]> clouds)
    {
        int count = clouds.Count;
        float[,] distances = new float[count, count];

        for (int i = 0; i < count; i++)
            for (int j = i + 1; j < count; j++)
                distances[i, j] = distances[j, i] = PointCloudRecognizer.Distance(clouds[i], clouds[j]);

        List<int> chosen = new List<int>();
        float[] nearest = Enumerable.Repeat(float.PositiveInfinity, count).ToArray();

        while (chosen.Count < Mathf.Min(PerSymbol, count))
        {
            int best = -1;
            float bestCost = float.PositiveInfinity;

            for (int candidate = 0; candidate < count; candidate++)
            {
                if (chosen.Contains(candidate))
                    continue;

                float cost = 0f;
                for (int i = 0; i < count; i++)
                    cost += Mathf.Min(nearest[i], distances[candidate, i]);

                if (cost < bestCost)
                {
                    bestCost = cost;
                    best = candidate;
                }
            }

            chosen.Add(best);
            for (int i = 0; i < count; i++)
                nearest[i] = Mathf.Min(nearest[i], distances[best, i]);
        }

        return chosen;
    }
}
