using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WallAbility", menuName = "Custom/Ability/WallAbility")]
public class WallAbility : Ability
{
    private void OnEnable()
    {
        description = "Summon a wall by drawing a square.";
    }

    public void SpawnWall(Vector2[] points, MonoBehaviour runner,
        LineRenderer secondaryLineRenderer,
        Material groundMaterial, Material trippyTransparentMaterial)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.position = Vector3.zero;


        PolygonCollider2D polyCollider = wall.AddComponent<PolygonCollider2D>();
        polyCollider.points = points;


        Vector2 min = points[0];
        Vector2 max = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            min = Vector2.Min(min, points[i]);
            max = Vector2.Max(max, points[i]);
        }

        Vector2 center = (min + max) / 2f;
        Vector2 size = max - min;

        BoxCollider2D boxCollider = wall.AddComponent<BoxCollider2D>();
        boxCollider.offset = center;
        boxCollider.size = size;

        Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        Debug.Log("Wall spawned using drawn square!");
        if (secondaryLineRenderer != null && groundMaterial != null)
        {
            secondaryLineRenderer.material = groundMaterial;
        }

        runner.StartCoroutine(DestroyWallAfterTime(wall, activeTime, secondaryLineRenderer, trippyTransparentMaterial));
    }

    private IEnumerator DestroyWallAfterTime(GameObject wall, float delay,
        LineRenderer secondaryLineRenderer,
        Material trippyTransparentMaterial)
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