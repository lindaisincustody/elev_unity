using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
[RequireComponent(typeof(PolygonCollider2D), typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(ShadowCaster2D))]
public class RoomShape : MonoBehaviour
{
    [SerializeField] private string sortingLayer = "Frontground";
    [SerializeField] private int sortingOrder = 100;

    private const int MaxEdgePoints = 32;

    private static readonly int EdgePointsId = Shader.PropertyToID("_EdgePoints");
    private static readonly int EdgeCountId = Shader.PropertyToID("_EdgeCount");

    private Mesh generated;
    private int shapeHash;

    private void OnEnable()
    {
        Rebuild();
    }

    private void OnDisable()
    {
        if (generated != null)
            DestroyImmediate(generated);
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (CurrentShapeHash() != shapeHash)
            Rebuild();
    }

    [Button]
    public void Rebuild()
    {
        PolygonCollider2D shape = GetComponent<PolygonCollider2D>();
        shape.isTrigger = true;

        Mesh mesh = shape.CreateMesh(false, false);
        mesh.name = name + "_RoomShape";

        Vector3[] vertices = mesh.vertices;
        Vector3 shift = (Vector3)PathCenter(shape) - mesh.bounds.center;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] += shift;

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        Bounds bounds = mesh.bounds;
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(
                Mathf.InverseLerp(bounds.min.x, bounds.max.x, vertices[i].x),
                Mathf.InverseLerp(bounds.min.y, bounds.max.y, vertices[i].y));
        }

        mesh.uv = uvs;

        if (generated != null)
            DestroyImmediate(generated);

        generated = mesh;
        shapeHash = CurrentShapeHash();

        GetComponent<MeshFilter>().sharedMesh = mesh;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = sortingLayer;
        renderer.sortingOrder = sortingOrder;

        PushEdgePoints(shape, renderer);

        ShadowCaster2D caster = GetComponent<ShadowCaster2D>();
        caster.castingOption = ShadowCaster2D.ShadowCastingOptions.SelfShadow;
    }

    private void PushEdgePoints(PolygonCollider2D shape, MeshRenderer renderer)
    {
        Vector2[] path = shape.GetPath(0);
        int count = Mathf.Min(path.Length, MaxEdgePoints);
        Vector4[] points = new Vector4[MaxEdgePoints];

        for (int i = 0; i < count; i++)
            points[i] = shape.offset + path[i];

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetVectorArray(EdgePointsId, points);
        block.SetInt(EdgeCountId, count);
        renderer.SetPropertyBlock(block);
    }


    private Vector2 PathCenter(PolygonCollider2D shape)
    {
        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.negativeInfinity;

        for (int path = 0; path < shape.pathCount; path++)
        {
            foreach (Vector2 point in shape.GetPath(path))
            {
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }
        }

        return (min + max) * 0.5f + shape.offset;
    }

    private int CurrentShapeHash()
    {
        PolygonCollider2D shape = GetComponent<PolygonCollider2D>();
        int hash = shape.pathCount;

        for (int path = 0; path < shape.pathCount; path++)
        {
            foreach (Vector2 point in shape.GetPath(path))
                hash = hash * 31 + point.GetHashCode();
        }

        return hash * 31 + shape.offset.GetHashCode();
    }
}
