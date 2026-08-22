using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class DrawingZoneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LetterDrawing letterDrawing;
    [Tooltip("Canvas that owns this element. Needed for Screen Space Camera mode. " +
             "Leave empty — Awake fills it automatically.")]
    [SerializeField] private Canvas canvas;

    [Header("Reveal")]
    [SerializeField] private float revealSpeed = 4f;
    [SerializeField] private float hideSpeed   = 2f;

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed  = 0.55f;  // UV units per second (0→2 range)

    private Material      _mat;
    private RectTransform _rect;
    private float         _revealAmount;
    private float         _pulseDiag;
    private bool          _wasDrawing;   // diagnostic: track drawing state changes

    void Awake()
    {
        _rect = GetComponent<RectTransform>();

        var img = GetComponent<RawImage>();
        // Instance the material so we never modify the shared asset.
        _mat = Instantiate(img.material);
        img.material = _mat;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        bool drawing = letterDrawing != null && letterDrawing.IsDrawing;
        // ── Diagnostic: log whenever drawing state flips ──────────────────
        if (drawing != _wasDrawing)
        {
            Debug.Log($"[DZC] drawing={drawing}  ld={letterDrawing?.gameObject.name ?? "NULL"}" +
                      $"  revealAmt={_revealAmount:F3}  matNull={_mat == null}");
            _wasDrawing = drawing;
        }

        // ── Reveal amount ────────────────────────────────────────────────
        float target = drawing ? 1f : 0f;
        float speed  = drawing ? revealSpeed : hideSpeed;
        _revealAmount = Mathf.MoveTowards(_revealAmount, target, speed * Time.deltaTime);
        _mat.SetFloat("_RevealAmount", _revealAmount);

        // ── Diagonal pulse (cycles 0 → 2 continuously) ───────────────────
        _pulseDiag = (_pulseDiag + pulseSpeed * Time.deltaTime) % 2f;
        _mat.SetFloat("_PulseDiag", _pulseDiag);

        // ── Finger UV ────────────────────────────────────────────────────
        // Use RectTransformUtility so canvas scale, safe-area insets,
        // and Screen Space Camera mode are all handled automatically.
        if (drawing && letterDrawing.DrawingScreenPos != Vector2.zero)
        {
            Camera uiCam = (canvas != null &&
                            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                           ? canvas.worldCamera : null;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rect, letterDrawing.DrawingScreenPos, uiCam, out Vector2 local))
            {
                Rect r = _rect.rect;
                float u = Mathf.Clamp01((local.x - r.xMin) / r.width);
                float v = Mathf.Clamp01((local.y - r.yMin) / r.height);
                _mat.SetVector("_FingerUV", new Vector4(u, v, 0f, 0f));
                _mat.SetFloat("_FingerActive", 1f);
            }
        }
        else
        {
            _mat.SetFloat("_FingerActive", 0f);
        }
    }

    void OnDestroy()
    {
        if (_mat != null) Destroy(_mat);
    }
}
