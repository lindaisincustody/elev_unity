using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron : MonoBehaviour
{
    public Transform neuron;
    public float curveStrength = 2.0f; // Adjust the curvature
    public int numPoints = 20; // Number of points on each curve
    public Transform[] targetObjects; // Assign target objects in the inspector
    private LineRenderer[] lineRenderers;
    private Vector3[] curveVariations;
    public float[] maxDistances; // Array of distance thresholds
    public Material lineRendererMat;


    void Start()
    {
        if (targetObjects.Length < 4)
        {
            Debug.LogError("Please assign at least 4 target objects.");
            return;
        }

        lineRenderers = new LineRenderer[targetObjects.Length];
        curveVariations = new Vector3[targetObjects.Length];

        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] != null)
            {
                curveVariations[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                lineRenderers[i] = CreateLineRenderer(targetObjects[i]);
                lineRenderers[i].material = lineRendererMat;
                UpdateLineRenderer(lineRenderers[i], targetObjects[i], curveVariations[i]);
            }
        }
    }

    void Update()
    {
        UpdateTargetPositions();

        for (int i = 0; i < lineRenderers.Length; i++)
        {
            if (lineRenderers[i] != null && targetObjects[i] != null)
            {
                UpdateLineRenderer(lineRenderers[i], targetObjects[i], curveVariations[i]);
            }
        }
    }

    void UpdateTargetPositions()
    {
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] != null && i < maxDistances.Length)
            {
                float distance = Vector2.Distance(neuron.transform.position, targetObjects[i].position);
                if (distance > maxDistances[i])
                {
                    Vector3 direction = (neuron.transform.position - targetObjects[i].position).normalized;

                    // Teleport to the opposite side of the neuron
                    Vector3 newPosition = neuron.transform.position + direction * maxDistances[i];

                    // Keep the original Z position unchanged
                    newPosition.z = targetObjects[i].position.z;

                    targetObjects[i].position = newPosition;
                }
            }
        }
    }


    LineRenderer CreateLineRenderer(Transform target)
    {
        GameObject lineObj = new GameObject("CurveLineTo" + target.name);
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = numPoints;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineObj.transform.SetParent(target);
        return lineRenderer;
    }

    void UpdateLineRenderer(LineRenderer lineRenderer, Transform target, Vector3 curveVariation)
    {
        Vector3 startPoint = neuron.transform.position;
        Vector3 endPoint = target.position;
        Vector3 direction = (endPoint - startPoint).normalized;
        Vector3 controlPoint1 = startPoint + Vector3.Cross(direction, curveVariation) * curveStrength;
        Vector3 controlPoint2 = endPoint + Vector3.Cross(direction, curveVariation) * curveStrength;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (numPoints - 1.0f);
            Vector3 point = CalculateBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
            lineRenderer.SetPosition(i, point);
        }
    }



    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}
