using System;
using System.Collections;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class LetterDrawing : MonoBehaviour
{
    public enum DrawingMode { Predicting, PaintingPath }
    public DrawingMode drawingMode = DrawingMode.PaintingPath;

    private IDrawingState currentState;

    [Header("General")]
    [SerializeField] public LineRenderer lineRenderer;
    [SerializeField] public LineRenderer secondaryLineRenderer;


    [Header("Predictions")]
    [SerializeField] public Material groundMaterial;
    [SerializeField] public Material trippyTransparentMaterial;
    [SerializeField] public NNModel model;
    [SerializeField] public RawImage renderTextureDisplay;
    [SerializeField] public TextMeshProUGUI currentLetterText;
    [SerializeField] public TextMeshProUGUI poemTextDisplay;
    [SerializeField, TextArea] public string poem = "Your poem text goes here.";

    [Header("Path creating")]


    private PaintingPathDrawingState paintingPathDrawingState;
    private PredictingDrawingState predictingDrawingState;

    void Start()
    {
        predictingDrawingState = new PredictingDrawingState(this);
        paintingPathDrawingState = new PaintingPathDrawingState();

        ChangeState();
    }

    private void ChangeState()
    {
        switch (drawingMode)
        {
            case DrawingMode.Predicting:
                currentState = predictingDrawingState;
                break;
            case DrawingMode.PaintingPath:
                currentState = paintingPathDrawingState;
                break;
            default:
                currentState = predictingDrawingState;
                break;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(1))
        {
            AddPoint();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            currentState.ProcessDrawing(lineRenderer, secondaryLineRenderer);
        }
    }

    private void StartDrawing()
    {
        lineRenderer.positionCount = 0;
        if (secondaryLineRenderer != null)
            secondaryLineRenderer.positionCount = 0;
    }

    private void AddPoint()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (lineRenderer.positionCount > 0)
        {
            Vector3 lastPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            mousePosition = Vector3.Lerp(lastPosition, mousePosition, 0.5f);
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePosition);

        if (secondaryLineRenderer != null)
        {
            secondaryLineRenderer.positionCount++;
            secondaryLineRenderer.SetPosition(secondaryLineRenderer.positionCount - 1, mousePosition);
        }
    }

    public Vector2[] GetDrawnPoints()
    {
        int pointCount = lineRenderer.positionCount;
        if (pointCount < 3)
        {
            Debug.LogWarning("Not enough points drawn to form a path.");
            return null;
        }
        Vector2[] points = new Vector2[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 pos = lineRenderer.GetPosition(i);
            points[i] = new Vector2(pos.x, pos.y);
        }
        // Optionally, close the shape if not already closed.
        if (points[0] != points[pointCount - 1])
        {
            Vector2[] closedPoints = new Vector2[pointCount + 1];
            Array.Copy(points, closedPoints, pointCount);
            closedPoints[pointCount] = points[0];
            points = closedPoints;
        }
        return points;
    }
}
