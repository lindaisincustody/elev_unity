using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConsumable : MonoBehaviour
{
    public int showPathTime;
    private MazeGenerator mazeGenerator;
    private BoxCollider2D collider2D;
    private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        mazeGenerator = FindFirstObjectByType<MazeGenerator>();
        collider2D = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mazeGenerator.Activate(showPathTime);
            collider2D.enabled = false;
            renderer.enabled = false;
        }
    }
}
