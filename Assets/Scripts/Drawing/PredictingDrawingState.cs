using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PredictingDrawingState : IDrawingState
{
    private LetterDrawing letterDrawing;

    public Prediction prediction;

    private RenderTexture mlRT;          // 96×96 B&W texture used for inference
    private Camera        mlCamera;      // moves to tightly frame each stroke for capture
    private RenderTexture displayRT;     // screen-resolution texture shown as UI overlay
    private Camera        displayCamera; // fixed, never moves — positions strokes in screen space
    private IWorker worker;

    private ParticleSystem sparkleInstance;
    private Coroutine feedbackRoutine;

    private string[] labels = new string[]
{
    "_Aries", "_Capricorn", "_Cross", "_EyesDollar", "_Heart", "_Leo", "_Mercury",
    "_Moon", "_Rightarrow", "_Sigma", "_Taurus", "_alpha", "_bigtriangleup",
    "_bowtie", "_boxplus", "_circlearrowleft", "_clubsuit", "_diagup",
    "_diamondsuit", "_downarrow", "_emptyset", "_female", "_infty", "_lambda",
    "_lightning", "_ltimes", "_male", "_psi", "_sim", "_spadesuit", "_square",
    "_star", "_textasteriskcentered", "_textcent", "_textgamma",
    "_textmusicalnote", "_theta", "_varphi"
};

    private readonly Dictionary<string, string> latexToUnicode = new Dictionary<string, string>
{
    { "_Aries", "♈" },
    { "_Capricorn", "♑" },
    { "_Cross", "†" },
    { "_EyesDollar", "🤑" },
    { "_Heart", "♥" },
    { "_Leo", "♌" },
    { "_Mercury", "☿" },
    { "_Moon", "☾" },
    { "_Rightarrow", "⇒" },
    { "_Sigma", "Σ" },
    { "_Taurus", "♉" },
    { "_alpha", "α" },
    { "_bigtriangleup", "△" },
    { "_bowtie", "⧓" },
    { "_boxplus", "⊞" },
    { "_circlearrowleft", "↺" },
    { "_clubsuit", "♣" },
    { "_diagup", "/" },
    { "_diamondsuit", "♦" },
    { "_downarrow", "↓" },
    { "_emptyset", "∅" },
    { "_female", "♀" },
    { "_infty", "∞" },
    { "_lambda", "λ" },
    { "_lightning", "⚡" },
    { "_ltimes", "⋉" },
    { "_male", "♂" },
    { "_psi", "ψ" },
    { "_sim", "∼" },
    { "_spadesuit", "♠" },
    { "_square", "■" },
    { "_star", "★" },
    { "_textasteriskcentered", "∗" },
    { "_textcent", "¢" },
    { "_textgamma", "γ" },
    { "_textmusicalnote", "♪" },
    { "_theta", "θ" },
    { "_varphi", "φ" }
};

    private float threshold = 0.91f;
    private int currentLetterIndex = 0;

    [Serializable]
    public struct Prediction
    {
        public string predictedLabel;
        public float[] predicted;

        public void SetPrediction(Tensor t, string[] labels)
        {
            predicted = t.AsFloats();
            int predictedIndex = Array.IndexOf(predicted, predicted.Max());
            predictedLabel = (predictedIndex >= 0 && predictedIndex < labels.Length)
                ? labels[predictedIndex]
                : "Unknown";
            Debug.Log($"Predicted Symbol: {predictedLabel}");
        }
    }

    public DrawingMode Mode => DrawingMode.Predicting;
    public DrawingWorld World => DrawingWorld.Underworld;

    private Camera ZoneCamera =>
        letterDrawing.DrawZoneCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : letterDrawing.DrawZoneCanvas.worldCamera;

    public bool CanStartStrokeAt(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            letterDrawing.DrawZoneRect, screenPosition, ZoneCamera);
    }

    public Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            letterDrawing.DrawZoneRect, screenPosition, ZoneCamera, out Vector2 local);

        Rect zone = letterDrawing.DrawZoneRect.rect;

        return displayCamera.ViewportToWorldPoint(new Vector3(
            Mathf.Clamp01((local.x - zone.xMin) / zone.width),
            Mathf.Clamp01((local.y - zone.yMin) / zone.height),
            Mathf.Abs(displayCamera.transform.position.z)));
    }

    public PredictingDrawingState(LetterDrawing newLetterDrawing)
    {
        letterDrawing = newLetterDrawing;

        InitializeCamera();
        InitializeModel();
        InitializeFX();

        if (letterDrawing.renderTextureDisplay != null)
            letterDrawing.renderTextureDisplay.texture = mlRT;
    }

    public void Enter(LetterDrawing drawing)
    {
        letterDrawing = drawing;

        letterDrawing.CameraRig.SetActive(true);
        letterDrawing.ShowDrawZone(true);
        letterDrawing.drawingDisplay.texture = displayRT;
        letterDrawing.drawingDisplay.enabled = true;
    }

    public void Exit()
    {
        letterDrawing.CameraRig.SetActive(false);
        letterDrawing.drawingDisplay.enabled = false;
    }

    private void InitializeCamera()
    {
        mlCamera = letterDrawing.CameraRig.MLCamera;
        displayCamera = letterDrawing.CameraRig.DisplayCamera;

        mlRT = new RenderTexture(96, 96, 16, RenderTextureFormat.ARGB32);
        mlRT.Create();
        mlCamera.targetTexture = mlRT;

        Rect zone = letterDrawing.DrawZoneRect.rect;
        float camAspect = zone.width / zone.height;
        float scale = letterDrawing.DrawZoneCanvas.scaleFactor;
        int rtH = Mathf.Clamp(Mathf.RoundToInt(zone.height * scale), 64, 2160);
        int rtW = Mathf.Clamp(Mathf.RoundToInt(rtH * camAspect), 64, 4096);
        // Depth must be non-zero: the URP render graph rejects a camera target
        // texture whose Depth Stencil Format is None, even for a 2D-only camera.
        displayRT = new RenderTexture(rtW, rtH, 24, RenderTextureFormat.ARGB32);
        displayRT.Create();

        // camAspect matches the RT exactly — no stretch regardless of orientation.
        displayCamera.aspect = camAspect;
        displayCamera.targetTexture = displayRT;

        Debug.Log($"[PDS] InitializeCamera for '{letterDrawing.gameObject.name}'" +
                  $"  rtW={rtW}  rtH={rtH}  camAspect={camAspect:F3}" +
                  $"  displayRT={(displayRT != null ? "OK" : "NULL")}" +
                  $"  drawingDisplay={(letterDrawing.drawingDisplay != null ? "OK" : "NULL")}" +
                  $"  displayCamEnabled={displayCamera?.enabled}");
    }

    private void InitializeModel()
    {
        worker = ModelLoader.Load(letterDrawing.model).CreateWorker(WorkerFactory.Device.CPU);
        prediction = new Prediction();
    }

    public void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer)
    {
        EndDrawing();
        CenterDrawingInTexture();

        using var inputTensor = new Tensor(mlRT, 1);
        worker.Execute(inputTensor);
        Tensor output = worker.PeekOutput();

        prediction.SetPrediction(output, labels);

        float confidence = prediction.predicted.Max();

        if (confidence < threshold)
        {
            Debug.Log("Low confidence drawing, ignoring input.");
            Debug.Log(confidence);
            TriggerMissSparkle(secondaryLineRenderer);
            output.Dispose();
            return;
        }
        Debug.Log(confidence);
        MatchSymbolWithPoem(prediction.predictedLabel);
        TriggerCorrectSparkle(secondaryLineRenderer);
        DisplaySymbol(prediction.predictedLabel, secondaryLineRenderer);

        output.Dispose();
    }

    private void MatchSymbolWithPoem(string predictedSymbol)
    {
        string predictedLabel = prediction.predictedLabel;
        Debug.Log($"Predicted Symbol: {predictedLabel}");

        foreach (var enemy in EnemyManager.Instance.GetAllEnemies())
        {
            if (enemy.activeSymbols.Any(g =>
                    g.Glyph == (latexToUnicode.ContainsKey(predictedSymbol)
                        ? latexToUnicode[predictedSymbol]
                        : predictedSymbol)))
            {
                enemy.CheckSymbolMatch((latexToUnicode.ContainsKey(predictedSymbol)
                    ? latexToUnicode[predictedSymbol]
                    : predictedSymbol));
            }
        }

        AbilityActionRegistry.Instance.ExecuteAction(predictedLabel);
    }

    private void EndDrawing()
    {
        // Show only the primary (white) LR for the ML capture; hide the visual one.
        letterDrawing.lineRenderer.enabled = true;
        if (letterDrawing.secondaryLineRenderer != null)
            letterDrawing.secondaryLineRenderer.enabled = false;

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = mlRT;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = prev;
        mlCamera.Render();

        letterDrawing.lineRenderer.enabled = false;
        if (letterDrawing.secondaryLineRenderer != null)
            letterDrawing.secondaryLineRenderer.enabled = true;
    }

    private void CenterDrawingInTexture()
    {
        Bounds bounds = new Bounds(letterDrawing.lineRenderer.GetPosition(0), Vector3.zero);
        for (int i = 1; i < letterDrawing.lineRenderer.positionCount; i++)
            bounds.Encapsulate(letterDrawing.lineRenderer.GetPosition(i));

        Vector3 center = bounds.center;
        mlCamera.transform.position = new Vector3(center.x, center.y, mlCamera.transform.position.z);

        float marginFactor = 1.2f;
        float maxDrawingSize = Mathf.Max(bounds.size.x, bounds.size.y) * marginFactor;
        mlCamera.orthographicSize = Mathf.Max(maxDrawingSize / 2f, 0.01f);

        // Keep primary LR at the same absolute width as everywhere else.
        // widthMultiplier stays 1 so startWidth/endWidth are the sole source of truth.
        letterDrawing.lineRenderer.widthMultiplier = 1f;
        letterDrawing.lineRenderer.startWidth      = 0.10f;
        letterDrawing.lineRenderer.endWidth        = 0.10f;

        // Swap visibility: ML camera needs primary on, secondary off.
        letterDrawing.lineRenderer.enabled = true;
        if (letterDrawing.secondaryLineRenderer != null)
            letterDrawing.secondaryLineRenderer.enabled = false;

        mlCamera.Render();

        letterDrawing.lineRenderer.enabled = false;
        if (letterDrawing.secondaryLineRenderer != null)
            letterDrawing.secondaryLineRenderer.enabled = true;
    }

    private void DisplaySymbol(string label, LineRenderer lr)
    {
        string glyph = latexToUnicode.ContainsKey(label) ? latexToUnicode[label] : label;

        // Spawn above the player if a reference is set; fall back to stroke centre.
        Vector3 worldPos;
        if (letterDrawing.playerTransform != null)
        {
            worldPos = letterDrawing.playerTransform.position
                     + Vector3.up * letterDrawing.symbolVerticalOffset;
        }
        else
        {
            var pts = new Vector3[lr.positionCount];
            lr.GetPositions(pts);
            var b = new Bounds(pts[0], Vector3.zero);
            for (int i = 1; i < pts.Length; i++) b.Encapsulate(pts[i]);
            worldPos = b.center + Vector3.up * letterDrawing.symbolVerticalOffset;
        }

        float worldSize = letterDrawing.symbolScale;

        var tmp = GameObject.Instantiate(letterDrawing.symbolPrefab, worldPos, Quaternion.identity);
        tmp.text = glyph;
        tmp.transform.localScale = Vector3.zero;

        // Parent to the player so the glyph tracks movement during its lifetime.
        if (letterDrawing.playerTransform != null)
        {
            tmp.transform.SetParent(letterDrawing.playerTransform, worldPositionStays: false);
            tmp.transform.localPosition = Vector3.up * letterDrawing.symbolVerticalOffset;
        }

        Camera refCam = Camera.main;
        tmp.transform.rotation = refCam.transform.rotation;

        // When parented, localScale is in the parent's scale space.
        // Counter-act the player's lossy scale so the glyph always renders at
        // exactly symbolScale world units — even if the player sprite is
        // non-uniformly scaled for pixel-art sizing.
        Vector3 targetScale;
        if (letterDrawing.playerTransform != null)
        {
            Vector3 ls = letterDrawing.playerTransform.lossyScale;
            float sx = Mathf.Abs(ls.x) > 1e-4f ? worldSize / ls.x : worldSize;
            float sy = Mathf.Abs(ls.y) > 1e-4f ? worldSize / ls.y : worldSize;
            float sz = Mathf.Abs(ls.z) > 1e-4f ? worldSize / ls.z : worldSize;
            targetScale = new Vector3(sx, sy, sz);
        }
        else
        {
            targetScale = Vector3.one * worldSize;
        }

        var tmpMesh = tmp.GetComponent<TextMeshPro>();
        letterDrawing.StartCoroutine(
            ComplexStamp(tmpMesh.transform, tmpMesh, targetScale, letterDrawing.symbolStampDuration)
        );

        GameObject.Destroy(tmp.gameObject, letterDrawing.symbolLifetime);
    }

    private void InitializeFX()
    {
        if (letterDrawing.sparkleEffectPrefab != null)
        {
            sparkleInstance = GameObject.Instantiate(
                letterDrawing.sparkleEffectPrefab,
                Vector3.zero,
                Quaternion.identity
            );
            sparkleInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private IEnumerator FlashPop()
    {
        var mat = letterDrawing.secondaryLineRenderer.sharedMaterial;
        var originalColor = mat.GetColor("_Color");

        // Always animate from the inspector-defined base width, never from
        // the current widthMultiplier — which may be mid-animation if a rapid
        // second draw interrupted the previous coroutine before it could restore.
        float baseWidth = letterDrawing.drawStrokeWidth;
        float elapsed = 0f, total = letterDrawing.flashDuration;

        while (elapsed < total)
        {
            float norm = elapsed / total;
            float pulse = Mathf.Sin(norm * Mathf.PI);
            mat.SetColor("_Color", Color.Lerp(originalColor, Color.white, pulse));
            float animWidth = baseWidth * (1f + (letterDrawing.scalePop - 1f) * pulse);
            letterDrawing.secondaryLineRenderer.startWidth = animWidth;
            letterDrawing.secondaryLineRenderer.endWidth   = animWidth;

            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetColor("_Color", originalColor);
        letterDrawing.secondaryLineRenderer.startWidth = baseWidth;
        letterDrawing.secondaryLineRenderer.endWidth   = baseWidth;
    }
    private void AnimateSparkleAlong(LineRenderer lr)
    {
        var pts = new Vector3[lr.positionCount];
        lr.GetPositions(pts);
        letterDrawing.StartCoroutine(MoveSparkle(pts, letterDrawing.sparkleDuration));
    }

    private IEnumerator MoveSparkle(Vector3[] pts, float duration)
    {
        sparkleInstance.Play();

        float totalLen = 0f;
        for (int i = 1; i < pts.Length; i++)
            totalLen += Vector3.Distance(pts[i - 1], pts[i]);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float dist = t * totalLen;
            float acc = 0f;
            Vector3 pos = pts[0];
            for (int i = 1; i < pts.Length; i++)
            {
                float seg = Vector3.Distance(pts[i - 1], pts[i]);
                if (acc + seg >= dist)
                {
                    float subT = (dist - acc) / seg;
                    pos = Vector3.Lerp(pts[i - 1], pts[i], subT);
                    break;
                }
                acc += seg;
            }
            sparkleInstance.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        sparkleInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private IEnumerator ClearLinesAfter(float delay, int versionAtTrigger)
    {
        yield return new WaitForSeconds(delay);
        if (letterDrawing.drawVersion != versionAtTrigger)
            yield break;

        letterDrawing.lineRenderer.positionCount = 0;
        if (letterDrawing.secondaryLineRenderer != null)
            letterDrawing.secondaryLineRenderer.positionCount = 0;
    }

    private void TriggerCorrectSparkle(LineRenderer lr)
    {
        if (sparkleInstance == null) return;

        var mainModule = sparkleInstance.main;
        mainModule.startColor = letterDrawing.correctSparkleColor;

        AnimateSparkleAlong(lr);

        // flash and pop — reset width immediately so interrupted animations don't compound
        if (feedbackRoutine != null)
        {
            letterDrawing.StopCoroutine(feedbackRoutine);
            letterDrawing.secondaryLineRenderer.startWidth = letterDrawing.drawStrokeWidth;
            letterDrawing.secondaryLineRenderer.endWidth   = letterDrawing.drawStrokeWidth;
        }
        feedbackRoutine = letterDrawing.StartCoroutine(FlashPop());

        // pulse the line gradient
        letterDrawing.StartCoroutine(PulseLineAlong(lr, letterDrawing.sparkleDuration));

        int v = letterDrawing.drawVersion;
        letterDrawing.StartCoroutine(ClearLinesAfter(1f, v));
    }

    private void TriggerMissSparkle(LineRenderer lr)
    {
        if (sparkleInstance == null) return;

        var mainModule = sparkleInstance.main;
        mainModule.startColor = letterDrawing.missSparkleColor;

        AnimateSparkleAlong(lr);

   

        int v = letterDrawing.drawVersion;
        letterDrawing.StartCoroutine(ClearLinesAfter(1f, v));
    }

    private IEnumerator PulseLineAlong(LineRenderer lr, float duration)
    {
        Gradient orig = lr.colorGradient;
        float elapsed = 0f;
        float band = 0.1f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float half = band * 0.5f;
            float a = Mathf.Clamp01(t - half);
            float b = Mathf.Clamp01(t + half);

            var cks = new List<GradientColorKey>();
            var aks = new List<GradientAlphaKey>();

            Color c0 = orig.Evaluate(a);
            cks.Add(new GradientColorKey(c0, 0f));
            aks.Add(new GradientAlphaKey(c0.a, 0f));

            c0 = orig.Evaluate(a);
            cks.Add(new GradientColorKey(c0, a));
            aks.Add(new GradientAlphaKey(c0.a, a));

            cks.Add(new GradientColorKey(Color.white, t));
            aks.Add(new GradientAlphaKey(1f, t));

            c0 = orig.Evaluate(b);
            cks.Add(new GradientColorKey(c0, b));
            aks.Add(new GradientAlphaKey(c0.a, b));

            c0 = orig.Evaluate(1f);
            cks.Add(new GradientColorKey(c0, 1f));
            aks.Add(new GradientAlphaKey(c0.a, 1f));

            var g = new Gradient();
            g.SetKeys(cks.ToArray(), aks.ToArray());
            lr.colorGradient = g;

            elapsed += Time.deltaTime;
            yield return null;
        }

        lr.colorGradient = orig;
    }

    private IEnumerator ComplexStamp(Transform sym, TextMeshPro tmp, Vector3 targetScale, float duration)
    {
        float BackOut(float t)
        {
            const float s = 1.70158f;
            t = t - 1f;
            return t * t * ((s + 1f) * t + s) + 1f;
        }

        float elapsed = 0f;
        float wobbleFreq = 1f; 
        float rotAmp = 2.5f; 
        Color baseColor = tmp.color;

     
        tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        sym.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            
            float scaleVal = BackOut(t);
            sym.localScale = Vector3.one * (targetScale.x * scaleVal);
   
            float wobble = Mathf.Sin(t * Mathf.PI * wobbleFreq) * (1f - t) * rotAmp;
            sym.localRotation = Quaternion.Euler(0f, 0f, wobble);

            float a = Mathf.Clamp01(t / 0.3f) * baseColor.a;
            tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);

            elapsed += Time.deltaTime;
            yield return null;
        }

        sym.localScale = targetScale;
        sym.localRotation = Quaternion.identity;
        tmp.color = baseColor;
    }

    public void Dispose()
    {
        worker?.Dispose();
        worker = null;

        if (letterDrawing.drawingDisplay != null)
        {
            letterDrawing.drawingDisplay.enabled = false;
            letterDrawing.drawingDisplay.texture = null;
        }

        if (letterDrawing.CameraRig != null)
            GameObject.Destroy(letterDrawing.CameraRig.gameObject);

        mlCamera      = null;
        displayCamera = null;
        if (mlRT != null)          { mlRT.Release();      mlRT      = null; }
        if (displayRT != null)     { displayRT.Release(); displayRT = null; }
    }

}
