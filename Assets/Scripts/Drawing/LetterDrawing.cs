using System;
using System.Collections;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class LetterDrawing : Component
{
    public enum DrawingMode { Predicting, PaintingPath, PaintingFire }
    public DrawingMode drawingMode = DrawingMode.PaintingPath;
    private DrawingMode previousMode;

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

    [Header("Feedback FX")]
    public ParticleSystem sparkleEffectPrefab;
    [Tooltip("How long particles run before stopping")]
    public float sparkleDuration = 0.5f;
    [Tooltip("Coroutine flash + pop duration")]
    public float flashDuration = 0.4f;
    [Tooltip("Temporary scale multiplier on correct draw")]
    public float scalePop = 1.3f;
    [Tooltip("Color when you hit the right symbol")]
    public Color correctSparkleColor = Color.white;
    [Tooltip("Color when prediction fails")]
    public Color missSparkleColor = Color.red;

    private float maxDrawDistance = int.MaxValue;
    private float currentDrawDistance = 0f;

    private bool reachedMaxDistance = false;

    [HideInInspector] public int drawVersion = 0;

    private Action OnDraw;

    private PaintingPathDrawingState paintingPathDrawingState;
    private PredictingDrawingState predictingDrawingState;
    private PaintingFireDrawingState paintingFireDrawingState;

    void Start()
    {
        predictingDrawingState = new PredictingDrawingState(this);
        paintingPathDrawingState = new PaintingPathDrawingState();
        paintingFireDrawingState = new PaintingFireDrawingState();

        previousMode = drawingMode;
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
            case DrawingMode.PaintingFire:
                currentState = paintingFireDrawingState;
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
            OnDraw?.Invoke();
        }
    }

    private void StartDrawing()
    {
        drawVersion++;
        lineRenderer.positionCount = 0;
        if (secondaryLineRenderer != null)
            secondaryLineRenderer.positionCount = 0;

        currentDrawDistance = 0f;                // ← reset
        reachedMaxDistance = false;
    }

    private void AddPoint()
    {
        if (reachedMaxDistance) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 newPos = mousePosition;
        if (lineRenderer.positionCount > 0)
        {
            Vector3 lastPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            newPos = Vector3.Lerp(lastPosition, mousePosition, 0.5f);

            float segLen = Vector3.Distance(lastPosition, newPos);

            if (maxDrawDistance > 0f && currentDrawDistance + segLen > maxDrawDistance)
            {
                float allowed = maxDrawDistance - currentDrawDistance;
                Vector3 dir = (newPos - lastPosition).normalized;
                newPos = lastPosition + dir * allowed;
                reachedMaxDistance = true;
            }

            currentDrawDistance += Vector3.Distance(lastPosition, newPos);
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);

        if (secondaryLineRenderer != null)
        {
            secondaryLineRenderer.positionCount++;
            secondaryLineRenderer.SetPosition(secondaryLineRenderer.positionCount - 1, newPos);
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
        // Close the shape if not already closed.
        if (points[0] != points[pointCount - 1])
        {
            Vector2[] closedPoints = new Vector2[pointCount + 1];
            Array.Copy(points, closedPoints, pointCount);
            closedPoints[pointCount] = points[0];
            points = closedPoints;
        }
        return points;
    }

    public void ActivateFireState(float duration, float damageInterval, int damage, float maxDist, LayerMask layerMask, Material material)
    {
        drawingMode = DrawingMode.PaintingFire;
        OnDraw = RevertToPreviousState;
        maxDrawDistance = maxDist;
        paintingFireDrawingState.Setup(duration, damageInterval, damage, layerMask, material);
        ChangeState();
    }

    public void ChangeState(DrawingMode newMode)
    {
        previousMode = drawingMode;
        drawingMode = newMode;
        ChangeState();
    }

    public void RevertToPreviousState()
    {
        OnDraw = null;
        drawingMode = previousMode;
        maxDrawDistance = int.MaxValue;
        ChangeState();
    }
}
