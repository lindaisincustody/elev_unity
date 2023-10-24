using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private NavMeshAgent agent;

    public bool drawLine = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        lineRenderer.positionCount = 0; // Initially, no points in the line
    }

    private void Update()
    {
        if (agent.hasPath && drawLine)
        {
            Vector3[] pathPoints = agent.path.corners;

            // Update the LineRenderer positions to match the path waypoints
            lineRenderer.positionCount = pathPoints.Length;
            lineRenderer.SetPositions(pathPoints);
        }
        else
        {
            // Clear the LineRenderer if the agent has no path
            lineRenderer.positionCount = 0;
        }
    }


}
