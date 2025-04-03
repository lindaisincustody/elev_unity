using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingFireDrawingState : IDrawingState
{
    private GameObject lastPathColliderObj;

    public void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer)
    {
        int pointCount = mainLineRenderer.positionCount;
        if (pointCount < 2)
            return;

        Vector2[] pts = new Vector2[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 pos = mainLineRenderer.GetPosition(i);
            pts[i] = new Vector2(pos.x, pos.y);
        }

        float thickness = 0.5f;
        float halfThickness = thickness / 2f;

        List<Vector2> leftSide = new List<Vector2>();
        List<Vector2> rightSide = new List<Vector2>();

        for (int i = 0; i < pts.Length; i++)
        {
            Vector2 normal;

            if (i == 0)
            {
                Vector2 dir = (pts[i + 1] - pts[i]).normalized;
                normal = new Vector2(-dir.y, dir.x);
            }
            else if (i == pts.Length - 1)
            {
                Vector2 dir = (pts[i] - pts[i - 1]).normalized;
                normal = new Vector2(-dir.y, dir.x);
            }
            else
            {
                Vector2 dir1 = (pts[i] - pts[i - 1]).normalized;
                Vector2 dir2 = (pts[i + 1] - pts[i]).normalized;
                Vector2 normal1 = new Vector2(-dir1.y, dir1.x);
                Vector2 normal2 = new Vector2(-dir2.y, dir2.x);
                normal = (normal1 + normal2).normalized;
            }

            leftSide.Add(pts[i] + normal * halfThickness);
            rightSide.Add(pts[i] - normal * halfThickness);
        }

        List<Vector2> polygonPoints = new List<Vector2>(leftSide);
        rightSide.Reverse();
        polygonPoints.AddRange(rightSide);

        if (lastPathColliderObj != null)
            GameObject.Destroy(lastPathColliderObj);

        GameObject pathColliderObj = new GameObject("PathCollider");
        pathColliderObj.AddComponent<DrawnFireTrigger>();
        pathColliderObj.transform.position = Vector3.zero;

        PolygonCollider2D polygonCollider = pathColliderObj.AddComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;
        polygonCollider.points = polygonPoints.ToArray();   

        lastPathColliderObj = pathColliderObj;
    }
}
