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
    [Tooltip("RawImage overlay on top of DrawingZoneGrid — PredictingDrawingState fills its texture at runtime.")]
    [SerializeField] public RawImage drawingDisplay;
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

    /// <summary>
    /// Fixed screen-space camera created by PredictingDrawingState.
    /// Points are placed in its world space so they never drift when the
    /// gameplay camera moves with the player.
    /// </summary>
    [HideInInspector] public Camera drawingCamera;

    private Action OnDraw;

    private PaintingPathDrawingState paintingPathDrawingState;
    private PredictingDrawingState predictingDrawingState;
    private PaintingFireDrawingState paintingFireDrawingState;

    private void Awake()
    {
        // Ensure both LineRenderers exist AND live at scene root with position (0,0,0).
        //
        // WHY scene root matters:
        //   P1 uses scene-root LineRenderer objects so their Transform.position = (0,0,0).
        //   The display camera sits at (0,0,-10) with orthoSize=5 and only covers the area
        //   near the world origin.  Unity computes LineRenderer.bounds from world vertices
        //   (useWorldSpace=true) but internally still uses the renderer's own Transform for
        //   camera frustum culling — if the LR is a child of Player(Clone) at the spawn
        //   point (e.g. (3,-5,0)) the bounds test fails and the display camera culls the
        //   LR even though the drawn stroke is inside its frustum.
        //   Moving the LR to scene root (position=0,0,0) matches P1's working setup exactly.
        //
        // If the prefab already has LRs wired as child objects, reparent them to root.
        // If they are null (scene refs that don't survive in the prefab), create fresh ones.

        if (lineRenderer == null)
        {
            var go = new GameObject("LineRenderer_Dynamic");
            // No SetParent — stays at scene root, position (0,0,0).
            lineRenderer = go.AddComponent<LineRenderer>();
            Debug.Log($"[LD] '{gameObject.name}': created dynamic primary LR at scene root.");
        }
        else if (lineRenderer.transform.parent != null)
        {
            // Already exists but is a prefab child — move to scene root.
            lineRenderer.transform.SetParent(null);
            lineRenderer.transform.position = Vector3.zero;
            Debug.Log($"[LD] '{gameObject.name}': moved primary LR to scene root (was child of '{lineRenderer.transform.parent?.name ?? "?"}').");
        }

        if (secondaryLineRenderer == null)
        {
            var go = new GameObject("SecondaryLineRenderer_Dynamic");
            secondaryLineRenderer = go.AddComponent<LineRenderer>();
            Debug.Log($"[LD] '{gameObject.name}': created dynamic secondary LR at scene root.");
        }
        else if (secondaryLineRenderer.transform.parent != null)
        {
            secondaryLineRenderer.transform.SetParent(null);
            secondaryLineRenderer.transform.position = Vector3.zero;
            Debug.Log($"[LD] '{gameObject.name}': moved secondary LR to scene root (was child).");
        }
    }

    void Start()
    {
        predictingDrawingState = new PredictingDrawingState(this);
        paintingPathDrawingState = new PaintingPathDrawingState();
        paintingFireDrawingState = new PaintingFireDrawingState();

        previousMode = drawingMode;
        ChangeState();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        if (drawingCamera != null)
        {
            drawingCamera.gameObject.SetActive(true);
            drawingCamera.enabled = true;

            Debug.Log($"[LD] '{gameObject.name}' re-enabled drawingCamera '{drawingCamera.name}'.");
        }
    }

    // NOTE: Do NOT call EnhancedTouchSupport.Disable() here.
    // EnhancedTouchSupport is a global, non-reference-counted flag shared by
    // every component in the process (TouchJoystick, TouchDrawController, ...).
    // Calling Disable() here would kill touch input for all of them. The
    // subsystem stays enabled for the lifetime of the scene, which is harmless.
    private void OnDisable()
    {
        // This LetterDrawing may have created a display/drawing camera before it
        // was disabled. If we leave that camera alive, URP can still render it
        // even though this LetterDrawing no longer updates, letting a stale
        // camera steal the render slot/depth from the active one.
        if (drawingCamera != null)
        {
            drawingCamera.enabled = false;
            drawingCamera.gameObject.SetActive(false);

            Debug.Log($"[LD] '{gameObject.name}' disabled drawingCamera '{drawingCamera.name}'.");
        }
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

    /// <summary>Add a point to the active stroke, given a screen-space position (mouse OR touch).</summary>
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
            // Fallback for non-Predicting modes: use the gameplay camera.
            Camera cam = gameplayCamera != null ? gameplayCamera : Camera.main;
            if (cam == null) { Debug.LogError("LetterDrawing: no camera!"); return; }
            float depth = Mathf.Abs(cam.transform.position.z);
            worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
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