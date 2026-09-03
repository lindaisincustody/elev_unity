using System;
using UnityEngine;

public class DrawingStroke
{
    private readonly LineRenderer primary;

    private float minPointDistance;
    private float maxDistance;
    private float drawnDistance;
    private Vector3 lastPoint;

    public LineRenderer Visible { get; private set; }
    public bool ReachedMaxDistance { get; private set; }

    public DrawingStroke(LineRenderer primary)
    {
        this.primary = primary;
    }

    public void UseVisibleLine(LineRenderer line)
    {
        Visible = line;
        Clear();
    }

    public void Begin(float minPointDistance, float maxDistance)
    {
        this.minPointDistance = minPointDistance;
        this.maxDistance = maxDistance;

        Clear();
    }

    public void Clear()
    {
        primary.positionCount = 0;

        if (Visible != null)
            Visible.positionCount = 0;

        drawnDistance = 0f;
        ReachedMaxDistance = false;
        lastPoint = Vector3.positiveInfinity;
    }

    public void AddPoint(Vector3 worldPoint)
    {
        if (ReachedMaxDistance)
            return;

        worldPoint.z = 0f;

        if (float.IsInfinity(lastPoint.x))
        {
            Append(worldPoint);
            return;
        }

        if (Vector3.Distance(lastPoint, worldPoint) < minPointDistance)
            return;

        Vector3 point = Vector3.Lerp(lastPoint, worldPoint, 0.5f);
        float segment = Vector3.Distance(lastPoint, point);

        if (drawnDistance + segment > maxDistance)
        {
            float allowed = maxDistance - drawnDistance;
            point = lastPoint + (point - lastPoint).normalized * allowed;
            ReachedMaxDistance = true;
        }

        drawnDistance += Vector3.Distance(lastPoint, point);
        Append(point);
    }

    public Vector2[] ClosedPoints()
    {
        int count = primary.positionCount;
        if (count < 3)
            return null;

        Vector2[] points = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            Vector3 position = primary.GetPosition(i);
            points[i] = new Vector2(position.x, position.y);
        }

        if (points[0] == points[count - 1])
            return points;

        Array.Resize(ref points, count + 1);
        points[count] = points[0];
        return points;
    }

    private void Append(Vector3 point)
    {
        lastPoint = point;

        primary.positionCount++;
        primary.SetPosition(primary.positionCount - 1, point);

        if (Visible == null)
            return;

        Visible.positionCount++;
        Visible.SetPosition(Visible.positionCount - 1, point);
    }
}
