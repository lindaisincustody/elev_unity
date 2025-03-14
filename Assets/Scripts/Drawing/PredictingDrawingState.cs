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
            output.Dispose();
            capturedTexture.Apply();
            DebugInputTexture(capturedTexture);
            GameObject.Destroy(capturedTexture);
            return;
        }

        MatchSymbolWithPoem(prediction.predictedLabel);

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

}
