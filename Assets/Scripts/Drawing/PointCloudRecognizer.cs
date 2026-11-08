using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointCloudRecognizer
{
    public const float Threshold = 1.2f;

    private const int PointCount = 32;
    private const float RelevantMargin = 1.5f;

    private readonly List<Template> templates = new List<Template>();

    public void Add(string label, Vector2[] cloud, int source)
    {
        templates.Add(new Template(Array.IndexOf(SymbolRecognizer.Labels, label), cloud, source));
    }

    public void Clear()
    {
        templates.Clear();
    }

    public TemplateMatch Recognize(IReadOnlyList<Vector2> points, int excludedSource = -1)
    {
        Vector2[] cloud = Normalize(points);
        float[] distances = Enumerable.Repeat(float.PositiveInfinity, SymbolRecognizer.Labels.Length).ToArray();
        float best = float.PositiveInfinity;

        foreach (Template template in templates)
        {
            if (template.Source == excludedSource)
                continue;

            float limit = Mathf.Min(distances[template.Label], best + RelevantMargin);
            distances[template.Label] = Mathf.Min(distances[template.Label], Distance(cloud, template.Cloud, limit));
            best = Mathf.Min(best, distances[template.Label]);
        }

        return new TemplateMatch(distances);
    }

    public static Vector2[] Points(LineRenderer line)
    {
        Vector3[] positions = new Vector3[line.positionCount];
        line.GetPositions(positions);

        return positions.Select(position => (Vector2)position).ToArray();
    }

    public static Vector2[] Normalize(IReadOnlyList<Vector2> points)
    {
        Vector2[] resampled = Resample(points);

        Vector2 min = new Vector2(resampled.Min(p => p.x), resampled.Min(p => p.y));
        Vector2 max = new Vector2(resampled.Max(p => p.x), resampled.Max(p => p.y));
        float size = Mathf.Max(max.x - min.x, max.y - min.y, 1e-5f);

        Vector2 centroid = resampled.Aggregate(Vector2.zero, (sum, p) => sum + p) / resampled.Length;

        return resampled.Select(p => (p - centroid) / size).ToArray();
    }

    public static float Distance(Vector2[] a, Vector2[] b, float limit = float.PositiveInfinity)
    {
        int step = Mathf.FloorToInt(Mathf.Sqrt(a.Length));
        float min = limit;

        for (int start = 0; start < a.Length; start += step)
        {
            min = Mathf.Min(min, CloudDistance(a, b, start, min));
            min = Mathf.Min(min, CloudDistance(b, a, start, min));
        }

        return min;
    }

    private static float CloudDistance(Vector2[] a, Vector2[] b, int start, float limit)
    {
        int count = a.Length;
        Span<bool> matched = stackalloc bool[count];
        float sum = 0f;
        int i = start;

        for (int k = 0; k < count; k++)
        {
            float x = a[i].x;
            float y = a[i].y;
            float best = float.PositiveInfinity;
            int index = 0;

            for (int j = 0; j < count; j++)
            {
                if (matched[j])
                    continue;

                float dx = x - b[j].x;
                float dy = y - b[j].y;
                float squared = dx * dx + dy * dy;

                if (squared < best)
                {
                    best = squared;
                    index = j;
                }
            }

            matched[index] = true;
            sum += (1f - (float)k / count) * Mathf.Sqrt(best);

            if (sum >= limit)
                return sum;

            i = (i + 1) % count;
        }

        return sum;
    }

    private static Vector2[] Resample(IReadOnlyList<Vector2> points)
    {
        List<Vector2> source = points.ToList();
        float length = 0f;
        for (int i = 1; i < source.Count; i++)
            length += Vector2.Distance(source[i - 1], source[i]);

        float interval = length / (PointCount - 1);
        List<Vector2> result = new List<Vector2> { source[0] };
        float accumulated = 0f;

        for (int i = 1; i < source.Count; i++)
        {
            float distance = Vector2.Distance(source[i - 1], source[i]);

            if (distance > 0f && accumulated + distance >= interval)
            {
                Vector2 point = Vector2.Lerp(source[i - 1], source[i], (interval - accumulated) / distance);
                result.Add(point);
                source.Insert(i, point);
                accumulated = 0f;
            }
            else
            {
                accumulated += distance;
            }
        }

        while (result.Count < PointCount)
            result.Add(source[source.Count - 1]);

        return result.Take(PointCount).ToArray();
    }

    private readonly struct Template
    {
        public int Label { get; }
        public Vector2[] Cloud { get; }
        public int Source { get; }

        public Template(int label, Vector2[] cloud, int source)
        {
            Label = label;
            Cloud = cloud;
            Source = source;
        }
    }
}

public readonly struct TemplateMatch
{
    public float[] Distances { get; }
    public int Index { get; }

    public string Label => SymbolRecognizer.Labels[Index];
    public float Distance => Distances[Index];
    public bool IsAccepted => Distance <= PointCloudRecognizer.Threshold;

    public TemplateMatch(float[] distances)
    {
        Distances = distances;
        Index = Array.IndexOf(distances, distances.Min());
    }
}
