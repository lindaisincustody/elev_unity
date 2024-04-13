using UnityEngine;
using UnityEngine.Tilemaps;

public class BoxColliderTilemap : MonoBehaviour
{
    public Tilemap tilemap;  // Assign your tilemap in the inspector

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned.");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    // Correctly position the collider using the tilemap's local-to-world transformation
                    Vector3Int localPlace = new Vector3Int(x, y, 0);
                    Vector3 worldPosition = tilemap.CellToWorld(localPlace) + new Vector3(0.5f, 0.5f, 0);  // Centering the collider

                    AddBoxCollider(worldPosition);
                }
            }
        }
    }

    private void AddBoxCollider(Vector3 position)
    {
        GameObject colliderObject = new GameObject("Collider");
        colliderObject.transform.parent = tilemap.transform;
        colliderObject.transform.position = position;

        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true; // Or false, depending on your needs
        collider.size = new Vector2(1, 1); // This assumes your tiles are 1x1 units
    }
}