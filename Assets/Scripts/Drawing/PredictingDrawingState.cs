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
        "_Capricorn", "_Heart", "_Leo", "_Moon", "_Rightarrow", "_bowtie",
        "_clubsuit", "_descnode", "_diagup", "_diamond", "_downarrow",
        "_infty", "_ocircle", "_oplus", "_spadesuit", "_square", "_star",
        "_textgamma", "_textmusicalnote", "_varphi"
    };

    private readonly Dictionary<string, string> latexToUnicode = new Dictionary<string, string>
    {
        { "_Capricorn", "♑" },
        { "_Heart", "♥" },
        { "_Leo", "♌" },
        { "_Moon", "☾" },
        { "_Rightarrow", "⇒" },
        { "_bowtie", "⧓" },
        { "_clubsuit", "♣" },
        { "_descnode", "⤵" },
        { "_diagup", "/" },
        { "_diamond", "♦" },
        { "_downarrow", "↓" },
        { "_infty", "∞" },
        { "_ocircle", "⦾" },
        { "_oplus", "⊕" },
        { "_spadesuit", "♠" },
        { "_square", "■" },
        { "_star", "★" },
        { "_textgamma", "γ" },
        { "_textmusicalnote", "♪" },
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
        float threshold = 0.91f;

        if (confidence < threshold)
        {
            Debug.Log("Low confidence drawing, ignoring input.");
            TriggerMissSparkle(secondaryLineRenderer);
            output.Dispose();
            capturedTexture.Apply();
            DebugInputTexture(capturedTexture);
            GameObject.Destroy(capturedTexture);
            return;
        }

        MatchSymbolWithPoem(prediction.predictedLabel);
        TriggerCorrectSparkle(secondaryLineRenderer);

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
        // 1) Sparkles
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

        // 2) Flash + pop
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

        // color the particles
        var mainModule = sparkleInstance.main;
        mainModule.startColor = letterDrawing.correctSparkleColor;

        // animate sparkle along
        AnimateSparkleAlong(lr);

        // flash & pop
        if (feedbackRoutine != null)
            letterDrawing.StopCoroutine(feedbackRoutine);
        feedbackRoutine = letterDrawing.StartCoroutine(FlashPop());

        // pulse the line gradient
        letterDrawing.StartCoroutine(PulseLineAlong(lr, letterDrawing.sparkleDuration));

        // clear that version after a second
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

            // build 5-key gradient
            var cks = new List<GradientColorKey>();
            var aks = new List<GradientAlphaKey>();

            // before band
            Color c0 = orig.Evaluate(a);
            cks.Add(new GradientColorKey(c0, 0f));
            aks.Add(new GradientAlphaKey(c0.a, 0f));

            // start band
            c0 = orig.Evaluate(a);
            cks.Add(new GradientColorKey(c0, a));
            aks.Add(new GradientAlphaKey(c0.a, a));

            // mid band white
            cks.Add(new GradientColorKey(Color.white, t));
            aks.Add(new GradientAlphaKey(1f, t));

            // end band back to original
            c0 = orig.Evaluate(b);
            cks.Add(new GradientColorKey(c0, b));
            aks.Add(new GradientAlphaKey(c0.a, b));

            // after band
            c0 = orig.Evaluate(1f);
            cks.Add(new GradientColorKey(c0, 1f));
            aks.Add(new GradientAlphaKey(c0.a, 1f));

            var g = new Gradient();
            g.SetKeys(cks.ToArray(), aks.ToArray());
            lr.colorGradient = g;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // restore
        lr.colorGradient = orig;
    }

}
