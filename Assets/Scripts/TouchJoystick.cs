using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchJoystick : MonoBehaviour
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 100f;
    [SerializeField, Range(0f, 1f)] private float zoneEndX = 0.5f; // left half

    private Vector2 _direction;
    private bool _isActive;
    private int _activeFingerIndex = -1;

    public Vector2 Direction => _direction;
    public bool IsActive => _isActive;

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

    void OnFingerDown(Finger f)
    {
        if (_activeFingerIndex != -1) return;                       // already tracking a finger
        if (f.screenPosition.x > Screen.width * zoneEndX) return;   // outside left half

        _activeFingerIndex = f.index;
        _isActive = true;
        background.position = f.screenPosition;
        handle.position = f.screenPosition;
        background.gameObject.SetActive(true);
    }

    void OnFingerMove(Finger f)
    {
        if (f.index != _activeFingerIndex) return;
        Vector2 delta = f.screenPosition - (Vector2)background.position;
        delta = Vector2.ClampMagnitude(delta, handleRange);
        handle.position = (Vector2)background.position + delta;
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

    void Hide()
    {
        if (background != null) background.gameObject.SetActive(false);
    }
}