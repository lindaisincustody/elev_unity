using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAssigner : MonoBehaviour
{
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        AssignRandomBrightColor();
    }

    private void AssignRandomBrightColor()
    {
        // Generate random values for RGB components
        float r = Random.Range(0.2f, 1f); // Adjust the range based on your preference
        float g = Random.Range(0.2f, 1f);
        float b = Random.Range(0.2f, 1f);

        // Assign the bright color to the SpriteRenderer
        sprite.color = new Color(r, g, b);
    }
}
