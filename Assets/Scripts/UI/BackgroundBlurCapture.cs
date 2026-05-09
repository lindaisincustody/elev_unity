using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the same GameObject as the RawImage that shows the frosted-glass background.
///
/// When the lobby panel becomes visible (OnEnable) this script waits for the current
/// frame to finish rendering, then captures the scene camera into a small RenderTexture
/// and hands it to the RawImage's material as _MainTex.  The UI/FrostedGlassBlur shader
/// then blurs and tints the captured snapshot.
///
/// Requirements:
///   - The RawImage must have a Material using "UI/FrostedGlassBlur" assigned.
///   - sourceCamera should be your gameplay camera (defaults to Camera.main).
///   - Downscale = 4  →  capture at 1/4 resolution (free extra blur + fast).
/// </summary>
[RequireComponent(typeof(RawImage))]
public class BackgroundBlurCapture : MonoBehaviour
{
    [Tooltip("The camera whose view is captured. Defaults to Camera.main if left empty.")]
    [SerializeField] private Camera sourceCamera;

    [Tooltip("Capture resolution = Screen / downscale. 4 = quarter-res (recommended).")]
    [Range(1, 8)]
    [SerializeField] private int downscale = 4;

    private RenderTexture _captureRT;
    private RawImage      _rawImage;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();

        if (sourceCamera == null)
            sourceCamera = Camera.main;

        if (sourceCamera == null)
            Debug.LogError("[BackgroundBlurCapture] sourceCamera is null and Camera.main not found.");
    }

    private void OnEnable()
    {
        StartCoroutine(CaptureAfterFrame());
    }

    private void OnDestroy()
    {
        ReleaseRT();
    }

    // ── Capture ───────────────────────────────────────────────────────────────

    /// <summary>Refresh the blur snapshot (call this if the scene changes while the panel is open).</summary>
    public void Refresh() => StartCoroutine(CaptureAfterFrame());

    private IEnumerator CaptureAfterFrame()
    {
        // Wait until the camera has finished rendering this frame so we capture
        // a complete scene image (not a half-rendered one).
        yield return new WaitForEndOfFrame();

        if (sourceCamera == null) yield break;

        EnsureRT();

        // Temporarily redirect the camera to our RT, render once, restore.
        RenderTexture prev = sourceCamera.targetTexture;
        sourceCamera.targetTexture = _captureRT;
        sourceCamera.Render();
        sourceCamera.targetTexture = prev;

        // Hand the snapshot to the RawImage so the blur shader can sample it.
        _rawImage.texture = _captureRT;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void EnsureRT()
    {
        int w = Mathf.Max(1, Screen.width  / downscale);
        int h = Mathf.Max(1, Screen.height / downscale);

        // Re-create only when resolution changes (e.g. first call, or orientation flip).
        if (_captureRT != null && _captureRT.width == w && _captureRT.height == h)
            return;

        ReleaseRT();

        _captureRT             = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32);
        _captureRT.filterMode  = FilterMode.Bilinear;
        _captureRT.wrapMode    = TextureWrapMode.Clamp;
        _captureRT.Create();
    }

    private void ReleaseRT()
    {
        if (_captureRT == null) return;
        _captureRT.Release();
        Destroy(_captureRT);
        _captureRT = null;
    }
}
