using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingPathDrawingState : IDrawingState
{
    /// <summary>
    /// Processes the drawing by converting the drawn line into a thick path.
    /// It creates a polygon (with the specified thickness) and adds a PolygonCollider2D
    /// (set as a trigger) to a new GameObject.
    /// </summary>
    public void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer)
    {
        // Ensure there are enough points to form a path.
        int pointCount = mainLineRenderer.positionCount;
        if (pointCount < 2)
        {
            Debug.LogWarning("Not enough points to create a thick path.");
            return;
        }

        // Extract points from the LineRenderer (assuming the drawing is in world space on the XY plane).
        Vector2[] pts = new Vector2[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 pos = mainLineRenderer.GetPosition(i);
            pts[i] = new Vector2(pos.x, pos.y);
        }

        // Thickness of the path.
        float thickness = 0.5f; // Adjust thickness as needed.
        float halfThickness = thickness / 2f;

        // Lists to store offset points for each side of the line.
        List<Vector2> leftSide = new List<Vector2>();
        List<Vector2> rightSide = new List<Vector2>();

        // Compute the perpendicular offset for each point.
        for (int i = 0; i < pts.Length; i++)
        {
            Vector2 normal;

            if (i == 0)
            {
                // First point: use direction to the next point.
                Vector2 dir = (pts[i + 1] - pts[i]).normalized;
                normal = new Vector2(-dir.y, dir.x);
            }
            else if (i == pts.Length - 1)
            {
                // Last point: use direction from the previous point.
                Vector2 dir = (pts[i] - pts[i - 1]).normalized;
                normal = new Vector2(-dir.y, dir.x);
            }
            else
            {
                // For middle points: average the normals from previous and next segments.
                Vector2 dir1 = (pts[i] - pts[i - 1]).normalized;
                Vector2 dir2 = (pts[i + 1] - pts[i]).normalized;
                Vector2 normal1 = new Vector2(-dir1.y, dir1.x);
                Vector2 normal2 = new Vector2(-dir2.y, dir2.x);
                normal = (normal1 + normal2).normalized;
            }

            // Calculate offset points for both sides.
            leftSide.Add(pts[i] + normal * halfThickness);
            rightSide.Add(pts[i] - normal * halfThickness);
        }

        // Create a closed polygon by combining the left side and the reversed right side.
        List<Vector2> polygonPoints = new List<Vector2>(leftSide);
        rightSide.Reverse();
        polygonPoints.AddRange(rightSide);

        // Create a new GameObject to hold the collider.
        GameObject pathColliderObj = new GameObject("PathCollider");
        pathColliderObj.AddComponent<DrawnPathTrigger>();
        pathColliderObj.transform.position = Vector3.zero;

        // Add a PolygonCollider2D and set its points.
        PolygonCollider2D polygonCollider = pathColliderObj.AddComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;
        polygonCollider.points = polygonPoints.ToArray();

        Debug.Log("Thick path collider created with " + polygonPoints.Count + " points.");
    }
}
