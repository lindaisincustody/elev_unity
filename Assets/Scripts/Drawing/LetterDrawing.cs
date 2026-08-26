using System;
using System.Collections;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class LetterDrawing : Component
{
    public enum DrawingMode { Predicting, PaintingPath, PaintingFire }
    public DrawingMode drawingMode = DrawingMode.PaintingPath;
    private DrawingMode previousMode;

    private IDrawingState currentState;

    [Header("Predictions")]
    [SerializeField] public Material groundMaterial;
    [Tooltip("White URP Unlit material — captured by the ML render camera. Must be visible on the Drawing layer.")]
    [SerializeField] public Material primaryLineMaterial;
    [Tooltip("Your Custom/TrippyTransparent material — what the player sees as the stroke.")]
    [SerializeField] public Material trippyTransparentMaterial;
    [SerializeField] public NNModel model;

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

    [Header("Symbol Display")]
    [Tooltip("A TextMeshPro (Mesh) prefab")]
    public TextMeshPro symbolPrefab;
    [Tooltip("The player's Transform — symbol spawns above this position.")]
    public Transform playerTransform;
    [Tooltip("World-space size of the stamped glyph")]
    public float symbolScale = 1f;
    [Tooltip("How long the glyph stays on-screen")]
    public float symbolLifetime = 2f;
    [Tooltip("How many world units above the player the glyph appears")]
    public float symbolVerticalOffset = 1.5f;

    [Tooltip("How long the stamp animation lasts")]
    public float symbolStampDuration = 0.5f;

    [Header("Stroke")]
    [Tooltip("Visual line width in drawing-camera world units. " +
             "Drawing camera orthoSize=5 → 1 unit ≈ screen_height/10 px. " +
             "Start around 0.04–0.08 and tune visually.")]
    [SerializeField] public float drawStrokeWidth = 0.10f;
    [Tooltip("Minimum world-unit distance between consecutive stroke points. " +
             "Prevents dense point stacking when the finger barely moves.")]
    [SerializeField] public float minPointWorldDistance = 0.01f;

    [Header("Touch (mobile)")]
    [Tooltip("Touches that start with screen X below this fraction are ignored (joystick zone).")]
    [Range(0f, 1f)]
    [SerializeField] public float drawZoneStartX = 0.5f;

    // Tracks which finger is currently drawing. -1 means no finger.
    private int drawingFingerId = -1;

    public bool IsDrawing => drawingFingerId != -1;
    public Vector2 DrawingScreenPos { get; private set; }

    private float maxDrawDistance = int.MaxValue;
    private float currentDrawDistance = 0f;

    private bool reachedMaxDistance = false;
    private Vector3 lastAddedWorldPos;   // last world-space point added — avoids reading back from LR

    [HideInInspector] public int drawVersion = 0;

    public Camera drawingCamera { get; private set; }

    public LineRenderer lineRenderer { get; private set; }
    public LineRenderer secondaryLineRenderer { get; private set; }

    public RawImage drawingDisplay { get; private set; }
    public RawImage renderTextureDisplay  { get; private set; }

    private Action OnDraw;

    private PaintingPathDrawingState paintingPathDrawingState;
    private PredictingDrawingState predictingDrawingState;
    private PaintingFireDrawingState paintingFireDrawingState;

    public DrawingCameraRig CameraRig { get; private set; }

    private readonly PlayerDrawingRig drawingRig = new PlayerDrawingRig();

    private void Awake()
    {
        drawingRig.Spawn();

        lineRenderer = drawingRig.Primary;
        secondaryLineRenderer = drawingRig.Secondary;
        CameraRig = drawingRig.Cameras;
        drawingCamera = CameraRig.DisplayCamera;
    }

    void Start()
    {
        HeaderCanvas header = UIManager.Instance.Get<HeaderCanvas>();
        drawingDisplay = header.DrawingDisplay;
        renderTextureDisplay = header.RenderTextureDisplay;

        predictingDrawingState = new PredictingDrawingState(this);
        paintingPathDrawingState = new PaintingPathDrawingState();
        paintingFireDrawingState = new PaintingFireDrawingState();

        previousMode = drawingMode;
        ChangeState();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        drawingCamera.gameObject.SetActive(true);
        drawingCamera.enabled = true;
    }

    // NOTE: Do NOT call EnhancedTouchSupport.Disable() here.
    // EnhancedTouchSupport is a global, non-reference-counted flag shared by
    // every component in the process (TouchJoystick, TouchDrawController, ...).
    // Calling Disable() here would kill touch input for all of them. The
    // subsystem stays enabled for the lifetime of the scene, which is harmless.
    private void OnDisable()
    {
        if (drawingCamera == null)
            return;

        drawingCamera.enabled = false;
        drawingCamera.gameObject.SetActive(false);
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
            AddPointAt(Input.mousePosition);
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

        currentDrawDistance = 0f;
        reachedMaxDistance = false;
        lastAddedWorldPos = Vector3.positiveInfinity; // sentinel — no previous point yet
        // The display camera's clearFlags handle RT clearing each frame automatically.
    }

    private void AddPointAt(Vector2 screenPosition)
    {
        DrawingScreenPos = screenPosition;
        if (reachedMaxDistance) return;

        Vector3 worldPos;

        if (drawingCamera != null)
        {
            // Map the screen position into the FIXED drawing camera's world space.
            // Because this camera never moves, the stroke stays exactly where
            // the finger touched regardless of player movement.
            float zoneW = Mathf.Max((1f - drawZoneStartX) * Screen.width, 1f);
            float normX  = (screenPosition.x - Screen.width * drawZoneStartX) / zoneW;
            float normY  = screenPosition.y / Mathf.Max(Screen.height, 1f);
            float depth  = Mathf.Abs(drawingCamera.transform.position.z);
            worldPos = drawingCamera.ViewportToWorldPoint(new Vector3(normX, normY, depth));
        }
        else
        {
            float depth = Mathf.Abs(Camera.main.transform.position.z);
            worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
        }

        worldPos.z = 0;

        Vector3 newPos = worldPos;
        bool hasLast = !float.IsInfinity(lastAddedWorldPos.x);

        // Skip this sample if the finger hasn't moved far enough — prevents
        // point stacking that makes the displayed stroke look bloated.
        if (hasLast && Vector3.Distance(lastAddedWorldPos, worldPos) < minPointWorldDistance)
            return;
        if (hasLast)
        {
            // Lerp in pure world space — never touch GetPosition() to avoid
            // local-vs-world confusion when the renderer is on a moving object.
            newPos = Vector3.Lerp(lastAddedWorldPos, worldPos, 0.5f);

            float segLen = Vector3.Distance(lastAddedWorldPos, newPos);

            if (maxDrawDistance > 0f && currentDrawDistance + segLen > maxDrawDistance)
            {
                float allowed = maxDrawDistance - currentDrawDistance;
                Vector3 dir = (newPos - lastAddedWorldPos).normalized;
                newPos = lastAddedWorldPos + dir * allowed;
                reachedMaxDistance = true;
            }

            currentDrawDistance += Vector3.Distance(lastAddedWorldPos, newPos);
        }

        lastAddedWorldPos = newPos;

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

    private void OnDestroy()
    {
        predictingDrawingState?.Dispose();
    }
}