using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class PredictingDrawingState : IDrawingState
{
    private LetterDrawing letterDrawing;

    public Prediction prediction;

    // Private fields for ML prediction.
    private RenderTexture renderTexture;
    private Camera renderCamera;
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

    public PredictingDrawingState(LetterDrawing newLetterDrawing)
    {
        letterDrawing = newLetterDrawing;

        InitializeCamera();
        InitializeModel();
        InitializeFX();
        UpdatePoemDisplay();

        letterDrawing.lineRenderer.gameObject.layer = LayerMask.NameToLayer("Drawing");
        renderCamera.cullingMask = LayerMask.GetMask("Drawing");

        Material trippyMaterial = new Material(Shader.Find("Unlit/Color"));
        Material trippyMaterialSecondary = new Material(Shader.Find("Custom/TrippyTransparent"));
        trippyMaterial.SetColor("_Color", Color.red);
        trippyMaterial.SetFloat("_Transparency", 0.5f);
        trippyMaterial.SetFloat("_TimeSpeed", 1.0f);
        trippyMaterialSecondary.SetColor("_Color", Color.black);
        trippyMaterialSecondary.SetFloat("_Transparency", 0.5f);
        trippyMaterialSecondary.SetFloat("_TimeSpeed", 1.0f);
        letterDrawing.lineRenderer.material = trippyMaterial;
        letterDrawing.secondaryLineRenderer.material = trippyMaterialSecondary;

        if (letterDrawing.renderTextureDisplay != null)
        {
            letterDrawing.renderTextureDisplay.texture = renderTexture;
        }

        if (letterDrawing.groundMaterial != null)
        {
            letterDrawing.groundMaterial.mainTextureScale = new Vector2(10.0f, 0.5f);
        }

        var sec = letterDrawing.secondaryLineRenderer;
        Gradient initG = new Gradient();
        initG.SetKeys(
          new[] {
        new GradientColorKey( trippyMaterialSecondary.GetColor("_Color"), 0f ),
        new GradientColorKey( trippyMaterialSecondary.GetColor("_Color"), 1f )
          },
          new[] {
        new GradientAlphaKey(1f, 0f),
        new GradientAlphaKey(1f, 1f)
          }
        );
        sec.colorGradient = initG;
    }

    private void InitializeCamera()
    {
        renderTexture = new RenderTexture(96, 96, 16, RenderTextureFormat.R8);
        renderCamera = new GameObject("Render Camera").AddComponent<Camera>();

        renderCamera.orthographic = true;
        renderCamera.cullingMask = LayerMask.GetMask("Drawing");
        renderCamera.backgroundColor = Color.black;
        renderCamera.clearFlags = CameraClearFlags.Color;
        renderCamera.targetTexture = renderTexture;

        renderCamera.orthographicSize = Camera.main.orthographicSize * 0.7f;
        renderCamera.transform.position =
            new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
    }

    private void InitializeModel()
    {
        worker = ModelLoader.Load(letterDrawing.model).CreateWorker(WorkerFactory.Device.GPU);
        prediction = new Prediction();
    }

    private void UpdatePoemDisplay()
    {
        while (currentLetterIndex < letterDrawing.poem.Length &&
               (letterDrawing.poem[currentLetterIndex] == ' ' || !char.IsLetter(letterDrawing.poem[currentLetterIndex])))
        {
            currentLetterIndex++;
        }

        if (currentLetterIndex < letterDrawing.poem.Length)
        {
            string currentTargetLetter = letterDrawing.poem[currentLetterIndex].ToString();
            letterDrawing.currentLetterText.text = $"{currentTargetLetter}";
        }
        else
        {
            letterDrawing.currentLetterText.text = "Poem completed!";
        }

        string before = letterDrawing.poem.Substring(0, currentLetterIndex);
        string highlighted = currentLetterIndex < letterDrawing.poem.Length
            ? $"<color=#000000><b>{letterDrawing.poem[currentLetterIndex]}</b></color>"
            : "";
        string after = currentLetterIndex + 1 < letterDrawing.poem.Length
            ? $"<color=#00000080>{letterDrawing.poem.Substring(currentLetterIndex + 1)}</color>"
            : "";

        letterDrawing.poemTextDisplay.text = before + highlighted + after;
    }

    public void ProcessDrawing(LineRenderer mainLineRenderer, LineRenderer secondaryLineRenderer)
    {
        EndDrawing();
        CenterDrawingInTexture();

        Texture2D capturedTexture = new Texture2D(96, 96, TextureFormat.R8, false);
        RenderTexture.active = renderTexture;
        capturedTexture.ReadPixels(new Rect(0, 0, 96, 96), 0, 0);

        using var inputTensor = new Tensor(renderTexture, 1);
        worker.Execute(inputTensor);
        Tensor output = worker.PeekOutput();

        prediction.SetPrediction(output, labels);

        float confidence = prediction.predicted.Max();
        float threshold = 8.5f;

        if (confidence < threshold)
        {
            Debug.Log("Low confidence drawing, ignoring input.");
            Debug.Log(confidence);
            TriggerMissSparkle(secondaryLineRenderer);
            output.Dispose();
            capturedTexture.Apply();
            DebugInputTexture(capturedTexture);
            GameObject.Destroy(capturedTexture);
            return;
        }
        Debug.Log(confidence);
        MatchSymbolWithPoem(prediction.predictedLabel);
        TriggerCorrectSparkle(secondaryLineRenderer);
        DisplaySymbol(prediction.predictedLabel, secondaryLineRenderer);

        output.Dispose();
        capturedTexture.Apply();
        DebugInputTexture(capturedTexture);
        GameObject.Destroy(capturedTexture);
    }

    private void DebugInputTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes("DebugInput.png", bytes);
        Debug.Log("Input texture saved for debugging: DebugInput.png");
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
        renderCamera.Render();
    }

    private void CenterDrawingInTexture()
    {
        Bounds bounds = new Bounds(letterDrawing.lineRenderer.GetPosition(0), Vector3.zero);
        for (int i = 1; i < letterDrawing.lineRenderer.positionCount; i++)
        {
            bounds.Encapsulate(letterDrawing.lineRenderer.GetPosition(i));
        }

        Vector3 center = bounds.center;
        renderCamera.transform.position = new Vector3(center.x, center.y, renderCamera.transform.position.z);

        float marginFactor = 1.2f;
        float maxDrawingSize = Mathf.Max(bounds.size.x, bounds.size.y) * marginFactor;
        renderCamera.orthographicSize = maxDrawingSize / 2f;


        float baseWidth = 0.1f;
        float zoomLevel = renderCamera.orthographicSize;
        float widthMultiplier = Mathf.Clamp(10f / zoomLevel, 5f, 9f);
        letterDrawing.lineRenderer.widthMultiplier = baseWidth * widthMultiplier;

        if (letterDrawing.secondaryLineRenderer != null)
        {
            letterDrawing.secondaryLineRenderer.widthMultiplier = letterDrawing.lineRenderer.widthMultiplier * 0.8f;
        }

        renderCamera.Render();
    }

    private void DisplaySymbol(string label, LineRenderer lr)
    {

        string glyph = latexToUnicode.ContainsKey(label) ? latexToUnicode[label] : label;

        var pts = new Vector3[lr.positionCount];
        lr.GetPositions(pts);
        var bounds = new Bounds(pts[0], Vector3.zero);
        for (int i = 1; i < pts.Length; i++)
            bounds.Encapsulate(pts[i]);
        Vector3 worldPos = bounds.center + Vector3.up * letterDrawing.symbolVerticalOffset;

        float worldSize = Mathf.Max(bounds.size.x, bounds.size.y) * letterDrawing.symbolScale;

        var tmp = GameObject.Instantiate(letterDrawing.symbolPrefab, worldPos, Quaternion.identity);
        tmp.text = glyph;
        tmp.transform.localScale = Vector3.zero;

        tmp.transform.rotation = Camera.main.transform.rotation;

        Vector3 targetScale = Vector3.one * worldSize;
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

    private void TriggerFeedback(LineRenderer secondaryLR)
    {
        // sparkles
        if (sparkleInstance != null)
        {
            var pts = new Vector3[secondaryLR.positionCount];
            secondaryLR.GetPositions(pts);
            sparkleInstance.transform.position = pts[pts.Length - 1];
            sparkleInstance.Play();
            letterDrawing.StartCoroutine(StopSparkle());
            int thisVersion = letterDrawing.drawVersion;
            letterDrawing.StartCoroutine(ClearLinesAfter(1f, thisVersion));
            AnimateSparkleAlong(secondaryLR);
        }

        // flash + pop
        if (feedbackRoutine != null)
            letterDrawing.StopCoroutine(feedbackRoutine);
        feedbackRoutine = letterDrawing.StartCoroutine(FlashPop());

        
    }

    private IEnumerator StopSparkle()
    {
        yield return new WaitForSeconds(letterDrawing.sparkleDuration);
        sparkleInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private IEnumerator FlashPop()
    {
        var mat = letterDrawing.secondaryLineRenderer.material;
        var originalColor = mat.GetColor("_Color");
        var originalWidth = letterDrawing.secondaryLineRenderer.widthMultiplier;
        float elapsed = 0f, total = letterDrawing.flashDuration;

        while (elapsed < total)
        {
            float norm = elapsed / total;
            float pulse = Mathf.Sin(norm * Mathf.PI);
            mat.SetColor("_Color", Color.Lerp(originalColor, Color.white, pulse));
            letterDrawing.secondaryLineRenderer.widthMultiplier =
                originalWidth * (1 + (letterDrawing.scalePop - 1) * pulse);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetColor("_Color", originalColor);
        letterDrawing.secondaryLineRenderer.widthMultiplier = originalWidth;
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

        // flash and pop
        if (feedbackRoutine != null)
            letterDrawing.StopCoroutine(feedbackRoutine);
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
    }

}
