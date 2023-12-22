using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform target;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target != null)
        {
            lineRenderer.SetPosition(0, transform.position); // Set the line's start point to the current object's position
            lineRenderer.SetPosition(1, target.position);   // Set the line's end point to the target's position
        }
    }
}
