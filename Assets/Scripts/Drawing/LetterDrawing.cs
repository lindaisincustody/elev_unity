using System;
using System.Collections;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class LetterDrawing : Component
{
    public enum DrawingMode { Predicting, PaintingPath, PaintingFire }
    public DrawingMode drawingMode = DrawingMode.PaintingPath;
    private DrawingMode previousMode;

    private IDrawingState currentState;
    [Header("Camera (assign in Inspector!)")]
    [Tooltip("The gameplay camera that renders the world. Do NOT rely on Camera.main.")]
    [SerializeField] public Camera gameplayCamera;

    [Header("Debug")]
    [SerializeField] private Text debugText;   // optional UI Text for live touch values
    [Header("General")]
    [SerializeField] public LineRenderer lineRenderer;
    [SerializeField] public LineRenderer secondaryLineRenderer;


    [Header("Predictions")]
    [SerializeField] public Material groundMaterial;
    [Tooltip("White URP Unlit material — captured by the ML render camera. Must be visible on the Drawing layer.")]
    [SerializeField] public Material primaryLineMaterial;
    [Tooltip("Your Custom/TrippyTransparent material — what the player sees as the stroke.")]
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

    [Header("Symbol Display")]
    [Tooltip("A TextMeshPro (Mesh) prefab")]
    public TextMeshPro symbolPrefab;
    [Tooltip("Multiplier for how large the glyph is relative to your draw bounds")]
    public float symbolScale = 1f;
    [Tooltip("How long the glyph stays on-screen")]
    public float symbolLifetime = 2f;
    [Tooltip("How much higher than the stroke center to place the glyph")]
    public float symbolVerticalOffset = 0.5f;

    [Tooltip("How long the stamp animation lasts")]
    public float symbolStampDuration = 0.5f;

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

    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();

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
        if (debugText != null)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Screen: {Screen.width}x{Screen.height}");
            sb.AppendLine($"Orient: {Screen.orientation}");
            Camera cam = gameplayCamera != null ? gameplayCamera : Camera.main;
            if (cam != null)
            {
                sb.AppendLine($"Cam: {cam.name}  Ortho={cam.orthographic}  Size={cam.orthographicSize:0.0}");
                sb.AppendLine($"CamPixels: {cam.pixelWidth}x{cam.pixelHeight}");
            }
            sb.AppendLine($"DrawZoneStart X: {Screen.width * drawZoneStartX:0}");
            foreach (var t in EnhancedTouch.activeTouches)
                sb.AppendLine($"T{t.touchId}: ({t.screenPosition.x:0},{t.screenPosition.y:0}) ph={t.phase}");
            debugText.text = sb.ToString();
        }
        // -------- TOUCH (iPhone / iPad) -------------------------------------
        foreach (EnhancedTouch t in EnhancedTouch.activeTouches)
        {
            switch (t.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    // Only start drawing if no finger is already drawing AND
                    // the touch is on the right half (left half is joystick).
                    if (drawingFingerId == -1 &&
                        t.screenPosition.x >= Screen.width * drawZoneStartX)
                    {
                        drawingFingerId = t.touchId;
                        StartDrawing();
                        AddPointAt(t.screenPosition);
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    if (t.touchId == drawingFingerId)
                        AddPointAt(t.screenPosition);
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (t.touchId == drawingFingerId)
                    {
                        drawingFingerId = -1;
                        DrawingScreenPos = Vector2.zero;
                        currentState?.ProcessDrawing(lineRenderer, secondaryLineRenderer);
                        OnDraw?.Invoke();
                    }
                    break;
            }
        }

        // -------- MOUSE (kept so editor iteration still works) --------------
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

        currentDrawDistance = 0f;                // ← reset
        reachedMaxDistance = false;
    }

    /// <summary>Add a point to the active stroke, given a screen-space position (mouse OR touch).</summary>
    private void AddPointAt(Vector2 screenPosition)
    {
        DrawingScreenPos = screenPosition;
        if (reachedMaxDistance) return;

        Camera cam = gameplayCamera != null ? gameplayCamera : Camera.main;
        if (cam == null) { Debug.LogError("LetterDrawing: no camera!"); return; }

        // ScreenToWorldPoint handles the camera's viewport rect correctly on all platforms.
        float depth = Mathf.Abs(cam.transform.position.z);
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
        worldPos.z = 0;

        Vector3 newPos = worldPos;
        if (lineRenderer.positionCount > 0)
        {
            Vector3 lastPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            newPos = Vector3.Lerp(lastPosition, worldPos, 0.5f);

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

    private void OnDestroy()
    {
        predictingDrawingState?.Dispose();
    }
}