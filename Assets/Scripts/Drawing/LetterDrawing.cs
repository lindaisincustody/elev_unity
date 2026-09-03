using System.Collections.Generic;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class LetterDrawing : Component
{
    [Header("Predictions")]
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
    [SerializeField] public float drawStrokeWidth = 0.10f;
    [SerializeField] public float minPointWorldDistance = 0.01f;
    [HideInInspector] public int drawVersion = 0;

    public RectTransform DrawZoneRect { get; private set; }
    public Canvas DrawZoneCanvas { get; private set; }

    public bool IsDrawing { get; private set; }
    public Vector2 DrawingScreenPos { get; private set; }

    public Camera drawingCamera { get; private set; }
    public LineRenderer lineRenderer { get; private set; }
    public LineRenderer secondaryLineRenderer { get; private set; }
    public RawImage drawingDisplay { get; private set; }
    public RawImage renderTextureDisplay { get; private set; }
    public DrawingCameraRig CameraRig { get; private set; }

    public Sprite CurrentSprite { get; private set; }
    public DrawingMode? CurrentMode { get; private set; }

    public event System.Action OnModeChanged;

    private readonly Dictionary<DrawingMode, IDrawingState> states = new Dictionary<DrawingMode, IDrawingState>();
    private readonly PlayerDrawingRig drawingRig = new PlayerDrawingRig();

    private Dictionary<DrawingMode, LineRenderer> modeLines;
    private HeaderCanvas header;
    private DrawingStroke stroke;
    private DrawingModeResolver resolver;
    private IDrawingState currentState;
    private bool stateApplied;

    private PredictingDrawingState predictingState;
    private PaintingFireDrawingState paintingFireState;
    private float abilityMaxDrawDistance = float.PositiveInfinity;

    private bool CanDraw => currentState != null;

    private DrawingWorld CurrentWorld =>
        SanityManager.Instance.IsPlayerInUnderworld ? DrawingWorld.Underworld : DrawingWorld.Overworld;

    private float MaxDrawDistance =>
        resolver.IsAbilityState(currentState) ? abilityMaxDrawDistance : float.PositiveInfinity;

    private void Awake()
    {
        drawingRig.Spawn();

        lineRenderer = drawingRig.Primary;

        CameraRig = drawingRig.Cameras;
        drawingCamera = CameraRig.DisplayCamera;

        modeLines = drawingRig.ModeLines;
        stroke = new DrawingStroke(lineRenderer);
        resolver = new DrawingModeResolver(states);
    }

    private void Start()
    {
        header = UIManager.Instance.Get<HeaderCanvas>();
        drawingDisplay = header.DrawingDisplay;
        renderTextureDisplay = header.RenderTextureDisplay;

        DrawZoneRect = header.DrawZone;
        DrawZoneCanvas = drawingDisplay.canvas;

        predictingState = new PredictingDrawingState(this);
        paintingFireState = new PaintingFireDrawingState();

        states[DrawingMode.Predicting] = predictingState;
        states[DrawingMode.PaintingPath] = new PaintingPathDrawingState();
        states[DrawingMode.PaintingFire] = paintingFireState;

        SanityManager.Instance.OnWorldChange += ResolveState;
        ResolveState();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        currentState?.Enter(this);
    }

    private void OnDisable()
    {
        if (CameraRig == null)
            return;

        CameraRig.SetActive(false);
    }

    private void OnDestroy()
    {
        SanityManager.Instance.OnWorldChange -= ResolveState;
        predictingState?.Dispose();
    }

    private void Update()
    {
        if (!CanDraw)
            return;

        if (Input.GetMouseButtonDown(1) && currentState.CanStartStrokeAt(Input.mousePosition))
            BeginStroke();
        else if (IsDrawing && Input.GetMouseButton(1))
            ExtendStroke(Input.mousePosition);
        else if (IsDrawing && Input.GetMouseButtonUp(1))
            EndStroke();
    }

    private void BeginStroke()
    {
        IsDrawing = true;
        drawVersion++;
        stroke.Begin(minPointWorldDistance, MaxDrawDistance);
    }

    private void ExtendStroke(Vector2 screenPosition)
    {
        DrawingScreenPos = screenPosition;
        stroke.AddPoint(currentState.ScreenToWorldPoint(screenPosition));
    }

    private void EndStroke()
    {
        IsDrawing = false;
        currentState.ProcessDrawing(lineRenderer, secondaryLineRenderer);

        if (!resolver.IsAbilityState(currentState))
            return;

        resolver.ClearAbility();
        abilityMaxDrawDistance = float.PositiveInfinity;
        ResolveState();
    }

    private void ResolveState()
    {
        if (states.Count == 0)
            return;

        IDrawingState resolved = resolver.Resolve(CurrentWorld);
        if (stateApplied && resolved == currentState)
            return;

        stateApplied = true;

        IsDrawing = false;
        stroke.Clear();

        currentState?.Exit();
        currentState = resolved;

        ApplyModeLine();

        if (currentState == null)
        {
            CameraRig.SetActive(false);
            ShowDrawZone(false);
            return;
        }

        currentState.Enter(this);
    }

    private void ApplyModeLine()
    {
        foreach (KeyValuePair<DrawingMode, LineRenderer> entry in modeLines)
            entry.Value.enabled = currentState != null && entry.Key == currentState.Mode;

        if (currentState == null)
        {
            CurrentMode = null;
            CurrentSprite = null;
            secondaryLineRenderer = null;
            stroke.UseVisibleLine(null);
        }
        else
        {
            CurrentMode = currentState.Mode;
            CurrentSprite = ConfigManager.Instance.Drawing.SpriteFor(currentState.Mode);
            secondaryLineRenderer = modeLines[currentState.Mode];
            stroke.UseVisibleLine(secondaryLineRenderer);
        }

        OnModeChanged?.Invoke();
    }

    public void ShowDrawZone(bool visible)
    {
        header.SetDrawZoneVisible(visible);
    }

    public void PushZoneMode(Object source, DrawingMode mode)
    {
        resolver.PushZone(source, mode);
        ResolveState();
    }

    public void PopZoneMode(Object source)
    {
        resolver.PopZone(source);
        ResolveState();
    }

    public Vector2[] GetDrawnPoints()
    {
        return stroke.ClosedPoints();
    }

    public void ActivateFireState(float duration, float damageInterval, int damage, float maxDist, LayerMask layerMask, Material material)
    {
        paintingFireState.Setup(duration, damageInterval, damage, layerMask, material);

        abilityMaxDrawDistance = maxDist;
        resolver.RequestAbility(DrawingMode.PaintingFire);
        ResolveState();
    }
}
