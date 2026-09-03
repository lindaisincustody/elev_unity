using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WallAbility", menuName = "Custom/Ability/WallAbility")]
public class WallAbility : Ability
{
    [Header("References")]
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public Material TrippyMaterial { get; private set; }

    [Header("Chain Effect")]
    [Tooltip("Shader to use for the chain effect")]
    [SerializeField] private Shader chainShader;
    [Tooltip("Tileable chain-of-0s (or whatever) texture")]
    [SerializeField] private Texture2D chainTexture;
    [Tooltip("Tint color for the chain")]
    [SerializeField] private Color chainTint = Color.cyan;
    [Tooltip("Scroll direction (UV space)")]
    [SerializeField] private Vector2 chainScroll = new Vector2(1, 0);
    [Tooltip("Scroll speed")]
    [SerializeField] private float chainSpeed = 0.8f;
    [Tooltip("Wave amplitude")]
    [SerializeField] private float chainWaveAmp = 0.15f;
    [Tooltip("Wave frequency")]
    [SerializeField] private float chainWaveFreq = 8f;

    public GameObject SpawnWall(Vector2[] points, MonoBehaviour runner,
        LineRenderer secondaryLineRenderer,
        Material groundMaterial, Material trippyTransparentMaterial)
    {
        Vector2 min = points[0];
        Vector2 max = points[0];
        foreach (Vector2 p in points)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Vector2 center = (min + max) / 2f;
        Vector2 size = max - min;

        GameObject wall = new GameObject("Wall");
        wall.transform.position = new Vector3(center.x, center.y, 0);

        float wallThickness = 0.2f;

        CreateWallEdge(wall.transform, new Vector2(size.x, wallThickness),
            new Vector2(0, size.y / 2f - wallThickness / 2f), groundMaterial, "TopEdge");
        CreateWallEdge(wall.transform, new Vector2(size.x, wallThickness),
            new Vector2(0, -(size.y / 2f - wallThickness / 2f)), groundMaterial, "BottomEdge");
        CreateWallEdge(wall.transform, new Vector2(wallThickness, size.y),
            new Vector2(-(size.x / 2f - wallThickness / 2f), 0), groundMaterial, "LeftEdge");
        CreateWallEdge(wall.transform, new Vector2(wallThickness, size.y),
            new Vector2(size.x / 2f - wallThickness / 2f, 0), groundMaterial, "RightEdge");

        Debug.Log("Wall spawned as a square border with visible edges!");

        runner.StartCoroutine(DestroyWallAfterTime(wall, activeTime, secondaryLineRenderer, trippyTransparentMaterial,
            null));

        return wall;
    }

    private void CreateWallEdge(Transform parent, Vector2 edgeSize, Vector2 localPosition,
        Material baseMaterial, string name)
    {
        GameObject edge = new GameObject(name);
        edge.transform.SetParent(parent);
        edge.transform.localPosition = localPosition;
        edge.layer = LayerMask.NameToLayer("BoxLayer");

        // collider + rigidbody
        var collider = edge.AddComponent<BoxCollider2D>();
        collider.size = edgeSize;
        var rb = edge.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // sprite renderer
        var sr = edge.AddComponent<SpriteRenderer>();

        // use your custom chain shader
        var mat = new Material(chainShader ?? Shader.Find("Custom/TrippyChain"));
        // assign the texture you drop in inspector
        mat.SetTexture("_MainTex", chainTexture);
        mat.SetColor("_Color", chainTint);
        mat.SetVector("_Scroll", chainScroll);
        mat.SetFloat("_Speed", chainSpeed);
        mat.SetFloat("_WaveAmp", chainWaveAmp);
        mat.SetFloat("_WaveFreq", chainWaveFreq);

        sr.material = mat;
        sr.sortingLayerName = "Frontground";
        sr.sortingOrder = 0;
        sr.drawMode = SpriteDrawMode.Sliced;
        // make a 1×1 sprite so slicing + tiling works
        sr.sprite = Sprite.Create(
            chainTexture,
            new Rect(0, 0, chainTexture.width, chainTexture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );
        sr.size = edgeSize;
    }


    private IEnumerator DestroyWallAfterTime(GameObject wall, float delay,
        LineRenderer secondaryLineRenderer,
        Material trippyTransparentMaterial, Action onWallDestroyed)
    {
        yield return new WaitForSeconds(delay);
        if (wall != null)
        {
            Destroy(wall);
            Debug.Log("Wall destroyed after active time.");
        }

        if (secondaryLineRenderer != null && trippyTransparentMaterial != null)
        {
            secondaryLineRenderer.material = trippyTransparentMaterial;
        }

        onWallDestroyed?.Invoke();
    }

    public override void Activate()
    {
        Debug.Log("Wall ability activated. Draw a square to spawn a wall.");
        OnActivate?.Invoke();
    }

    public override void Destroy()
    {
        OnCooldown?.Invoke();
    }
}