using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingFireDrawingState : PaintingDrawingState
{
    public override DrawingMode Mode => DrawingMode.PaintingFire;
    public override DrawingWorld World => DrawingWorld.Underworld;

    private float _duration;
    private int _damage;
    private float _interval;
    private Material _material;
    private LayerMask _layerMask;

    public void Setup(float duration, float interval, int damage, LayerMask layerMask, Material material)
    {
        _duration = duration;
        _interval = interval;
        _damage = damage;
        _material = material;
        _layerMask = layerMask;
    }

    public override void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer)
    {
        int pointCount = mainLineRenderer.positionCount;
        if (pointCount < 2)
            return;

        // 1) sample the center line
        Vector3[] centerPts3D = new Vector3[pointCount];
        Vector2[] centerPts2D = new Vector2[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 wpos = mainLineRenderer.GetPosition(i);
            centerPts3D[i] = wpos;
            centerPts2D[i] = new Vector2(wpos.x, wpos.y);
        }

        // 2) compute the thick corridor polygon as before
        float corridorWidth = 0.5f;
        float halfW = corridorWidth * 0.5f;

        var leftSide = new List<Vector2>();
        var rightSide = new List<Vector2>();

        for (int i = 0; i < centerPts2D.Length; i++)
        {
            Vector2 normal;
            if (i == 0)
            {
                Vector2 dir = (centerPts2D[i + 1] - centerPts2D[i]).normalized;
                normal = new Vector2(-dir.y, dir.x);
            }
            else if (i == centerPts2D.Length - 1)
            {
                Vector2 dir = (centerPts2D[i] - centerPts2D[i - 1]).normalized;
                normal = new Vector2(-dir.y, dir.x);
            }
            else
            {
                Vector2 d1 = (centerPts2D[i] - centerPts2D[i - 1]).normalized;
                Vector2 d2 = (centerPts2D[i + 1] - centerPts2D[i]).normalized;
                Vector2 n1 = new Vector2(-d1.y, d1.x);
                Vector2 n2 = new Vector2(-d2.y, d2.x);
                normal = (n1 + n2).normalized;
            }
            leftSide.Add(centerPts2D[i] + normal * halfW);
            rightSide.Add(centerPts2D[i] - normal * halfW);
        }

        var polygonPoints = new List<Vector2>(leftSide);
        rightSide.Reverse();
        polygonPoints.AddRange(rightSide);

        // 3) build the collider object
        GameObject pathColliderObj = new GameObject("PathCollider");
        pathColliderObj.transform.position = Vector3.zero;

        var fireTrigger = pathColliderObj.AddComponent<DrawnFireTrigger>();
        fireTrigger.duration = _duration;
        fireTrigger.damageInterval = _interval;
        fireTrigger.damage = _damage;
        fireTrigger.LayerMask = _layerMask;

        var polyCol = pathColliderObj.AddComponent<PolygonCollider2D>();
        polyCol.isTrigger = true;
        polyCol.points = polygonPoints.ToArray();

        // 4) add a brand-new LineRenderer **for the center line only**
        var lr = pathColliderObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = centerPts3D.Length;
        lr.loop = false;               // open line in the middle
        lr.startWidth = corridorWidth;       // full width so its half reaches to the side
        lr.endWidth = corridorWidth;
        lr.material = _material;
        // lr.alignment    = LineAlignment.View; // optional

        for (int i = 0; i < centerPts3D.Length; i++)
        {
            lr.SetPosition(i, centerPts3D[i]);
        }
    }
}
