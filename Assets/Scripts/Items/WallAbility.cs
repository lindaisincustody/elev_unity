using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WallAbility", menuName = "Custom/Ability/WallAbility")]
public class WallAbility : Ability
{
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

    private void CreateWallEdge(Transform parent, Vector2 edgeSize, Vector2 localPosition, Material baseMaterial,
        string name)
    {
        GameObject edge = new GameObject(name);
        edge.transform.SetParent(parent);
        edge.transform.localPosition = localPosition;
        edge.layer = LayerMask.NameToLayer("BoxLayer");
        BoxCollider2D collider = edge.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = edge.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        collider.size = edgeSize;

        SpriteRenderer sr = edge.AddComponent<SpriteRenderer>();
        Material edgeMat = new Material(baseMaterial);
        sr.material = edgeMat;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.red);
        tex.Apply();
        edgeMat.mainTexture = tex;
        sr.sortingLayerName = "Frontground";
        sr.sortingOrder = 0;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f), 100f);
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