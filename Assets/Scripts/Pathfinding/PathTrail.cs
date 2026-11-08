using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PathTrail : MonoBehaviour
{
    private const int CornerSteps = 8;
    private const float MinSegment = 0.0001f;
    private const float Far = 1000f;

    private static readonly int LengthId = Shader.PropertyToID("_Length");
    private static readonly int PixelsPerUnitId = Shader.PropertyToID("_PixelsPerUnit");

    [SerializeField] private SpriteRenderer field;
    [SerializeField] private SpriteRenderer sparkle;
    [SerializeField] private float pixelsPerUnit = 36f;
    [SerializeField] private float radius = 0.9f;
    [SerializeField] private float cornerRadius = 0.5f;
    [SerializeField] private float arriveDistance = 0.6f;
    [SerializeField] private float fadeDuration = 0.4f;

    private Transform watcher;
    private Vector2 destination;
    private float alpha;

    public bool HasPath { get; private set; }

    public event Action OnArrived;

    private void Awake()
    {
        SetAlpha(0f);
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
        Destroy(field.sprite.texture);
        Destroy(field.sprite);
    }

    public void Show(IReadOnlyList<Vector2> points, Vector2 to, Transform newWatcher)
    {
        destination = to;
        watcher = newWatcher;

        List<Vector2> path = Round(points);
        HasPath = path.Count > 1;

        Bake(path);
        sparkle.transform.position = Snap(to);

        Fade(1f);
    }

    public void Hide()
    {
        watcher = null;
        Fade(0f).OnComplete(() => Destroy(gameObject));
    }

    private void Update()
    {
        if (watcher == null || Vector2.Distance(watcher.position, destination) > arriveDistance)
            return;

        OnArrived?.Invoke();
        Hide();
    }

    private List<Vector2> Round(IReadOnlyList<Vector2> points)
    {
        List<Vector2> distinct = new List<Vector2>();

        foreach (Vector2 point in points)
        {
            if (distinct.Count == 0 || Vector2.Distance(distinct[distinct.Count - 1], point) > MinSegment)
                distinct.Add(point);
        }

        List<Vector2> rounded = new List<Vector2> { distinct[0] };

        for (int i = 1; i < distinct.Count - 1; i++)
        {
            Vector2 corner = distinct[i];
            Vector2 back = distinct[i - 1] - corner;
            Vector2 ahead = distinct[i + 1] - corner;
            float reach = Mathf.Min(cornerRadius, back.magnitude / 2f, ahead.magnitude / 2f);

            Vector2 start = corner + back.normalized * reach;
            Vector2 end = corner + ahead.normalized * reach;

            for (int step = 0; step <= CornerSteps; step++)
            {
                float t = step / (float)CornerSteps;
                float u = 1f - t;
                rounded.Add(u * u * start + 2f * u * t * corner + t * t * end);
            }
        }

        if (distinct.Count > 1)
            rounded.Add(distinct[distinct.Count - 1]);

        return rounded;
    }

    private void Bake(List<Vector2> path)
    {
        Vector2 min = path[0];
        Vector2 max = path[0];

        foreach (Vector2 point in path)
        {
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }

        Vector2Int origin = Vector2Int.FloorToInt((min - Vector2.one * radius) * pixelsPerUnit);
        Vector2Int corner = Vector2Int.CeilToInt((max + Vector2.one * radius) * pixelsPerUnit);
        int width = corner.x - origin.x;
        int height = corner.y - origin.y;

        float[] texels = new float[width * height * 2];
        float[] nearest = new float[width * height];

        for (int i = 0; i < nearest.Length; i++)
        {
            nearest[i] = radius * radius;
            texels[i * 2 + 1] = Far;
        }

        float along = 0f;
        float unit = 1f / pixelsPerUnit;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 a = path[i - 1];
            Vector2 b = path[i];
            float length = Vector2.Distance(a, b);

            if (length < MinSegment)
                continue;

            float dx = (b.x - a.x) / length;
            float dy = (b.y - a.y) / length;

            int rowFrom = Mathf.Max(0, Mathf.FloorToInt((Mathf.Min(a.y, b.y) - radius) * pixelsPerUnit) - origin.y);
            int rowTo = Mathf.Min(height, Mathf.CeilToInt((Mathf.Max(a.y, b.y) + radius) * pixelsPerUnit) - origin.y);

            for (int y = rowFrom; y < rowTo; y++)
            {
                float oy = (origin.y + y + 0.5f) * unit - a.y;
                float low = 0f;
                float high = length;

                if (Mathf.Abs(dy) > MinSegment)
                {
                    float enter = (oy - radius) / dy;
                    float exit = (oy + radius) / dy;
                    low = Mathf.Clamp(Mathf.Min(enter, exit), 0f, length);
                    high = Mathf.Clamp(Mathf.Max(enter, exit), 0f, length);
                }

                float left = a.x + dx * (dx < 0f ? high : low);
                float right = a.x + dx * (dx < 0f ? low : high);
                int columnFrom = Mathf.Max(0, Mathf.FloorToInt((left - radius) * pixelsPerUnit) - origin.x);
                int columnTo = Mathf.Min(width, Mathf.CeilToInt((right + radius) * pixelsPerUnit) - origin.x);

                for (int x = columnFrom; x < columnTo; x++)
                {
                    float ox = (origin.x + x + 0.5f) * unit - a.x;
                    float t = Mathf.Clamp(ox * dx + oy * dy, 0f, length);
                    float sx = ox - dx * t;
                    float sy = oy - dy * t;
                    float distance = sx * sx + sy * sy;
                    int index = y * width + x;

                    if (distance >= nearest[index])
                        continue;

                    nearest[index] = distance;
                    texels[index * 2] = along + t;
                    texels[index * 2 + 1] = Mathf.Sign(dx * oy - dy * ox) * Mathf.Sqrt(distance);
                }
            }

            along += length;
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGFloat, false, true)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixelData(texels, 0);
        texture.Apply(false, true);

        field.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero, pixelsPerUnit, 0, SpriteMeshType.FullRect);
        field.transform.position = (Vector2)origin / pixelsPerUnit;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        field.GetPropertyBlock(block);
        block.SetFloat(LengthId, along);
        block.SetFloat(PixelsPerUnitId, pixelsPerUnit);
        field.SetPropertyBlock(block);
    }

    private Vector2 Snap(Vector2 point)
    {
        return ((Vector2)Vector2Int.FloorToInt(point * pixelsPerUnit) + Vector2.one * 0.5f) / pixelsPerUnit;
    }

    private Tween Fade(float target)
    {
        DOTween.Kill(this);

        return DOTween.To(() => alpha, SetAlpha, target, fadeDuration).SetTarget(this);
    }

    private void SetAlpha(float value)
    {
        alpha = value;

        Color color = new Color(1f, 1f, 1f, value);
        field.color = color;
        sparkle.color = color;
    }
}
