using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Floating virtual joystick. Place this on a UI Image that covers the
/// left half of the Canvas (raycast target ON, alpha can be 0).
/// When the player taps the left half, the joystick "snaps" to that point
/// and follows the finger until released. Outputs a normalized Vector2
/// in <see cref="Direction"/>, which InputManager will read instead of WASD.
/// </summary>
[RequireComponent(typeof(Image))]
public class TouchJoystick : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References (children of the joystick zone)")]
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;

    [Header("Tuning")]
    [Tooltip("Max pixel distance the handle can travel from the background center.")]
    [SerializeField] private float handleRange = 80f;
    [Tooltip("Below this magnitude (0..1) input is treated as zero.")]
    [SerializeField] private float deadZone = 0.1f;
    [Tooltip("If true, the joystick visuals appear at the touch point and disappear on release.")]
    [SerializeField] private bool floating = true;

    private RectTransform baseRect;
    private Canvas canvas;
    private Camera uiCamera;
    private Vector2 input = Vector2.zero;
    private int activePointerId = -1;

    /// <summary>Normalized -1..1 direction. Vector2.zero when no finger.</summary>
    public Vector2 Direction => input;

    /// <summary>True while a finger is dragging this joystick.</summary>
    public bool IsActive => activePointerId != -1;

    private void Awake()
    {
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[TouchJoystick] Must be a child of a Canvas.");
            return;
        }

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;

        // Make the zone invisible but still receive raycasts.
        var zoneImage = GetComponent<Image>();
        zoneImage.color = new Color(0f, 0f, 0f, 0f);
        zoneImage.raycastTarget = true;

        if (floating && joystickBackground != null)
            joystickBackground.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Already tracking a finger? Ignore secondary touches in this zone.
        if (IsActive) return;

        activePointerId = eventData.pointerId;

        if (floating && joystickBackground != null)
        {
            joystickBackground.gameObject.SetActive(true);
            // Move the visuals to the touch point, in the joystick zone's local space.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                baseRect, eventData.position, uiCamera, out Vector2 localPoint);
            joystickBackground.anchoredPosition = localPoint;
        }

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId) return;
        if (joystickBackground == null || joystickHandle == null) return;

        Vector2 bgScreenPos = RectTransformUtility.WorldToScreenPoint(
            uiCamera, joystickBackground.position);

        Vector2 delta = eventData.position - bgScreenPos;
        delta = Vector2.ClampMagnitude(delta, handleRange);
        joystickHandle.anchoredPosition = delta;

        Vector2 normalized = delta / handleRange;
        input = normalized.magnitude < deadZone ? Vector2.zero : normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId) return;

        activePointerId = -1;
        input = Vector2.zero;

        if (joystickHandle != null)
            joystickHandle.anchoredPosition = Vector2.zero;
        if (floating && joystickBackground != null)
            joystickBackground.gameObject.SetActive(false);
    }
}