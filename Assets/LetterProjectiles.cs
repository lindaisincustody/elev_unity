using System.Collections.Generic;
using UnityEngine;

public class LetterProjectiles : MonoBehaviour
{
    private List<Vector3> shapeOffsets = new List<Vector3>();
    private Vector3 direction;
    public float speed = 5f;

    private LineRenderer lineRenderer;
    private Vector3 initialPosition; // Store initial position for offset calculations

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Initialize(List<Vector3> points)
    {
        shapeOffsets.Clear();

        // Store initial position and direction
        initialPosition = points[0];
        if (points.Count >= 2)
        {
            direction = (points[points.Count - 1] - points[points.Count - 2]).normalized;
        }
        else
        {
            direction = Vector3.right;
        }

        // Set the projectile's initial position
        transform.position = initialPosition;

        // Calculate offsets from the initial position
        foreach (var point in points)
        {
            shapeOffsets.Add(point - initialPosition);
        }

        // Set up LineRenderer with the offsets
        lineRenderer.positionCount = shapeOffsets.Count;
        for (int i = 0; i < shapeOffsets.Count; i++)
        {
            lineRenderer.SetPosition(i, shapeOffsets[i]);
        }
    }

    private void Update()
    {
        // Move the projectile in the set direction
        transform.position += direction * speed * Time.deltaTime;

        // Update LineRenderer positions to match the new position
        for (int i = 0; i < shapeOffsets.Count; i++)
        {
            lineRenderer.SetPosition(i, transform.position + shapeOffsets[i]);
        }
    }
}
