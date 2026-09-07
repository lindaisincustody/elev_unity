using System.Collections.Generic;
using UnityEngine;

public class PaintingPathDrawingState : PaintingDrawingState
{
    private const string PathLayer = "DrawingFX";

    public override DrawingMode Mode => DrawingMode.PaintingPath;
    public override DrawingWorld World => DrawingWorld.Overworld;

    private GameObject lastPathColliderObj;
    private MagicPathLine pathLine;

    public override void Enter(LetterDrawing drawing)
    {
        base.Enter(drawing);

        pathLine = drawing.secondaryLineRenderer.GetComponent<MagicPathLine>();
    }

    public override void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer)
    {
        int pointCount = mainLineRenderer.positionCount;
        if (pointCount < 2)
            return;

        Vector2[] points = new Vector2[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 position = mainLineRenderer.GetPosition(i);
            points[i] = new Vector2(position.x, position.y);
        }

        float halfThickness = pathLine.WalkableWidth * 0.5f;

        List<Vector2> leftSide = new List<Vector2>(pointCount);
        List<Vector2> rightSide = new List<Vector2>(pointCount);

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 normal;

            if (i == 0)
            {
                Vector2 direction = (points[i + 1] - points[i]).normalized;
                normal = new Vector2(-direction.y, direction.x);
            }
            else if (i == points.Length - 1)
            {
                Vector2 direction = (points[i] - points[i - 1]).normalized;
                normal = new Vector2(-direction.y, direction.x);
            }
            else
            {
                Vector2 incoming = (points[i] - points[i - 1]).normalized;
                Vector2 outgoing = (points[i + 1] - points[i]).normalized;
                Vector2 incomingNormal = new Vector2(-incoming.y, incoming.x);
                Vector2 outgoingNormal = new Vector2(-outgoing.y, outgoing.x);
                normal = (incomingNormal + outgoingNormal).normalized;
            }

            leftSide.Add(points[i] + normal * halfThickness);
            rightSide.Add(points[i] - normal * halfThickness);
        }

        rightSide.Reverse();
        leftSide.AddRange(rightSide);

        if (lastPathColliderObj != null)
            Object.Destroy(lastPathColliderObj);

        GameObject pathColliderObj = new GameObject("PathCollider");
        pathColliderObj.layer = LayerMask.NameToLayer(PathLayer);
        pathColliderObj.AddComponent<DrawnPathTrigger>();
        pathColliderObj.transform.position = Vector3.zero;

        PolygonCollider2D polygonCollider = pathColliderObj.AddComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;
        polygonCollider.points = leftSide.ToArray();

        lastPathColliderObj = pathColliderObj;
    }
}
