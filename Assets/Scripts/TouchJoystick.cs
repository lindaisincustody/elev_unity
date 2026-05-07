using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchJoystick : MonoBehaviour
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 100f;
    [SerializeField, Range(0f, 1f)] private float zoneEndX = 0.5f;
    [SerializeField] private Text debugText;   // optional — drag a UI Text here to see live values

    private Canvas _canvas;
    private RectTransform _canvasRect;
    private Vector2 _direction;
    private bool _isActive;
    private int _activeFingerIndex = -1;

    public Vector2 Direction => _direction;
    public bool IsActive => _isActive;

    void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasRect = _canvas.transform as RectTransform;
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.onFingerDown += OnFingerDown;
        ETouch.onFingerMove += OnFingerMove;
        ETouch.onFingerUp += OnFingerUp;
        Hide();
    }

    void OnDisable()
    {
        ETouch.onFingerDown -= OnFingerDown;
        ETouch.onFingerMove -= OnFingerMove;
        ETouch.onFingerUp -= OnFingerUp;
    }

    void Update()
    {
        if (debugText == null) return;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Screen: {Screen.width} x {Screen.height}");
        sb.AppendLine($"Orient: {Screen.orientation}");
        sb.AppendLine($"Half X: {Screen.width * zoneEndX:0}");
        foreach (var t in ETouch.activeTouches)
            sb.AppendLine($"T{t.ended}: ({t.screenPosition.x:0}, {t.screenPosition.y:0})");
        debugText.text = sb.ToString();
    }

    void OnFingerDown(Finger f)
    {
        if (_activeFingerIndex != -1) return;
        if (f.screenPosition.x > Screen.width * zoneEndX) return;

        _activeFingerIndex = f.index;
        _isActive = true;

        Vector2 local = ScreenToCanvas(f.screenPosition);
        background.anchoredPosition = local;
        handle.anchoredPosition = local;
        background.gameObject.SetActive(true);
    }

    void OnFingerMove(Finger f)
    {
        if (f.index != _activeFingerIndex) return;

        Vector2 local = ScreenToCanvas(f.screenPosition);
        Vector2 delta = local - background.anchoredPosition;
        delta = Vector2.ClampMagnitude(delta, handleRange);
        handle.anchoredPosition = background.anchoredPosition + delta;
        _direction = delta / handleRange;
    }

    void OnFingerUp(Finger f)
    {
        if (f.index != _activeFingerIndex) return;
        _activeFingerIndex = -1;
        _isActive = false;
        _direction = Vector2.zero;
        Hide();
    }

    Vector2 ScreenToCanvas(Vector2 screenPos)
    {
        Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPos, cam, out Vector2 local);
        return local;
    }

    void Hide()
    {
        if (background != null) background.gameObject.SetActive(false);
    }
}