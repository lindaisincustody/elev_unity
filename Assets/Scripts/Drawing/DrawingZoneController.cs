using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class DrawingZoneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LetterDrawing letterDrawing;

    [Header("Reveal")]
    [SerializeField] private float revealSpeed  = 4f;
    [SerializeField] private float hideSpeed    = 2f;

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed   = 0.55f;  // UV units per second (0→2 range)

    private Material _mat;
    private float    _revealAmount;
    private float    _pulseDiag;

    void Awake()
    {
        var img = GetComponent<RawImage>();
        // Instance the material so we never modify the shared asset.
        _mat = Instantiate(img.material);
        img.material = _mat;
    }

    void Update()
    {
        bool drawing = letterDrawing != null && letterDrawing.IsDrawing;

        // ── Reveal amount ────────────────────────────────────────────────
        float target = drawing ? 1f : 0f;
        float speed  = drawing ? revealSpeed : hideSpeed;
        _revealAmount = Mathf.MoveTowards(_revealAmount, target, speed * Time.deltaTime);
        _mat.SetFloat("_RevealAmount", _revealAmount);

        // ── Diagonal pulse (cycles 0 → 2 continuously) ───────────────────
        _pulseDiag = (_pulseDiag + pulseSpeed * Time.deltaTime) % 2f;
        _mat.SetFloat("_PulseDiag", _pulseDiag);

        // ── Finger UV ────────────────────────────────────────────────────
        if (drawing && letterDrawing.DrawingScreenPos != Vector2.zero)
        {
            // Map screen position into the drawing zone's UV space.
            // The draw zone occupies the right half of the screen
            // (drawZoneStartX → 1.0 in screen-X, full height in screen-Y).
            float zoneStartX = letterDrawing.drawZoneStartX;
            float screenFracX = letterDrawing.DrawingScreenPos.x / Screen.width;
            float screenFracY = letterDrawing.DrawingScreenPos.y / Screen.height;

            float zoneWidth = 1f - zoneStartX;
            float u = Mathf.Clamp01((screenFracX - zoneStartX) / Mathf.Max(zoneWidth, 0.001f));
            float v = Mathf.Clamp01(screenFracY);

            _mat.SetVector("_FingerUV", new Vector4(u, v, 0f, 0f));
            _mat.SetFloat("_FingerActive", 1f);
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
