using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Handles drawing strokes with any finger that lands in the "draw zone"
/// (right half of the screen by default). Works simultaneously with
/// <see cref="TouchJoystick"/> because the joystick lives on the left half
/// and consumes its own touches via the EventSystem; this controller reads
/// raw touches and ignores anything that falls inside the joystick zone.
///
/// Hook up <see cref="OnStrokeFinished"/> if you want to feed the points
/// into a shape recogniser (e.g. for LetterProjectile).
/// </summary>
public class TouchDrawController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera used to convert screen points into world space. Defaults to Camera.main.")]
    [SerializeField] private Camera drawCamera;
    [Tooltip("LineRenderer prefab cloned for each new stroke. Set Use World Space = true.")]
    [SerializeField] private LineRenderer linePrefab;

    [Header("Zone")]
    [Tooltip("Touches with screen X below this fraction are ignored (those belong to the joystick).")]
    [Range(0f, 1f)]
    [SerializeField] private float drawZoneStartX = 0.5f;

    [Header("Stroke tuning")]
    [Tooltip("Minimum world-space distance between consecutive points. Smooths the line.")]
    [SerializeField] private float minPointDistance = 0.05f;
    [Tooltip("Z plane in world space where points are placed (2D games usually 0).")]
    [SerializeField] private float drawZ = 0f;
    [Tooltip("If true, the visible line is destroyed when the finger lifts.")]
    [SerializeField] private bool clearOnRelease = true;
    [Tooltip("Delay before destroy when clearOnRelease is true. 0 = immediate.")]
    [SerializeField] private float clearDelay = 0.1f;

    /// <summary>Fired when a finger lifts. Receives the world-space points of that stroke.</summary>
    public System.Action<List<Vector3>> OnStrokeFinished;

    private class ActiveStroke
    {
        public LineRenderer line;
        public List<Vector3> points = new List<Vector3>();
    }

    private readonly Dictionary<int, ActiveStroke> strokes = new Dictionary<int, ActiveStroke>();

    private void Awake()
    {
        if (drawCamera == null)
        {
            drawCamera = Camera.main;
            if (drawCamera == null)
                Debug.LogError("[TouchDrawController] drawCamera is not assigned and Camera.main is null. Assign the gameplay camera in the Inspector.");
        }
        if (linePrefab == null)
            Debug.LogError("[TouchDrawController] linePrefab is not assigned.");
    }

    private void OnEnable() => EnhancedTouchSupport.Enable();

    // NOTE: Do NOT call EnhancedTouchSupport.Disable() here.
    // EnhancedTouchSupport is a global, non-reference-counted flag shared by every
    // component in the process (TouchJoystick, LetterDrawing, etc.).  Calling
    // Disable() here would kill touch input for all of them the moment this
    // component is disabled — including when the network spawn sequence
    // temporarily deactivates scene objects.  LetterDrawing and NetworkPlayerSync
    // balance the Enable/Disable lifecycle correctly; this script should only
    // enable, never disable.
    private void OnDisable() { }

    private void Update()
    {
        // Iterate over all active fingers; multi-touch friendly.
        foreach (EnhancedTouch t in EnhancedTouch.activeTouches)
        {
            bool inDrawZone = t.screenPosition.x >= Screen.width * drawZoneStartX;

            switch (t.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    if (inDrawZone) BeginStroke(t.touchId, t.screenPosition);
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    if (strokes.ContainsKey(t.touchId))
                        UpdateStroke(t.touchId, t.screenPosition);
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (strokes.ContainsKey(t.touchId))
                        EndStroke(t.touchId);
                    break;
            }
        }
    }

    private void BeginStroke(int id, Vector2 screenPos)
    {
        if (linePrefab == null) return;

        var stroke = new ActiveStroke
        {
            line = Instantiate(linePrefab)
        };
        stroke.line.positionCount = 0;
        stroke.line.useWorldSpace = true;
        strokes[id] = stroke;

        AddPoint(stroke, screenPos);
    }

    private void UpdateStroke(int id, Vector2 screenPos)
    {
        AddPoint(strokes[id], screenPos);
    }

    private void AddPoint(ActiveStroke stroke, Vector2 screenPos)
    {
        // Push the point a bit in front of the camera so ScreenToWorldPoint behaves predictably.
        float distFromCamera = Mathf.Abs(drawCamera.transform.position.z - drawZ);
        Vector3 world = drawCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, distFromCamera));
        world.z = drawZ;

        if (stroke.points.Count > 0 &&
            Vector3.Distance(stroke.points[stroke.points.Count - 1], world) < minPointDistance)
            return;

        stroke.points.Add(world);
        stroke.line.positionCount = stroke.points.Count;
        stroke.line.SetPosition(stroke.points.Count - 1, world);
    }

    private void EndStroke(int id)
    {
        var stroke = strokes[id];
        OnStrokeFinished?.Invoke(stroke.points);

        if (clearOnRelease && stroke.line != null)
        {
            if (clearDelay <= 0f) Destroy(stroke.line.gameObject);
            else Destroy(stroke.line.gameObject, clearDelay);
        }
        strokes.Remove(id);
    }
}