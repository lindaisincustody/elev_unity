using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LetterDrawing : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] public LineRenderer secondaryLineRenderer;
    [SerializeField] private NNModel _model;
    [SerializeField] private RawImage renderTextureDisplay;
    [SerializeField] private bool invert = true;
    [SerializeField] private TextMesh textMesh;
    [SerializeField] private GameObject heartParticlePrefab;

    [SerializeField] private TextMeshProUGUI poemTextDisplay;
    [SerializeField, TextArea] private string poem = "Your poem text goes here.";
    private int currentLetterIndex = 0;
    [SerializeField] private TextMeshProUGUI currentLetterText;

    [SerializeField] public Material groundMaterial;
    [SerializeField] public Material trippyTransparentMaterial;

    private GameObject activeWall = null;

    private bool isComplete = false;

    private Camera renderCamera;
    private RenderTexture renderTexture;
    private IWorker worker;
    private Coroutine heartStreamCoroutine;


    private readonly string[] _labels =
    {
        "_Capricorn",
        "_Heart",
        "_Leo",
        "_Moon",
        "_Rightarrow",
        "_bowtie",
        "_clubsuit",
        "_descnode",
        "_diagup",
        "_diamond",
        "_downarrow",
        "_infty",
        "_ocircle",
        "_oplus",
        "_spadesuit",
        "_square",
        "_star",
        "_textgamma",
        "_textmusicalnote",
        "_varphi"
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

    public Prediction prediction;

    void Start()
    {
        InitializeCamera();
        InitializeModel();
        UpdatePoemDisplay();

        lineRenderer.gameObject.layer = LayerMask.NameToLayer("Drawing");
        renderCamera.cullingMask = LayerMask.GetMask("Drawing");

        Material trippyMaterial = new Material(Shader.Find("Unlit/Color"));
        Material trippyMaterialSecondary = new Material(Shader.Find("Custom/TrippyTransparent"));
        trippyMaterial.SetColor("_Color", Color.red);
        trippyMaterial.SetFloat("_Transparency", 0.5f);
        trippyMaterial.SetFloat("_TimeSpeed", 1.0f);
        trippyMaterialSecondary.SetColor("_Color", Color.black);
        trippyMaterialSecondary.SetFloat("_Transparency", 0.5f);
        trippyMaterialSecondary.SetFloat("_TimeSpeed", 1.0f);
        lineRenderer.material = trippyMaterial;
        secondaryLineRenderer.material = trippyMaterialSecondary;

        if (renderTextureDisplay != null)
        {
            renderTextureDisplay.texture = renderTexture;
        }

        if (groundMaterial != null)
        {
            groundMaterial.mainTextureScale = new Vector2(10.0f, 0.5f);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(1))
        {
            AddPoint();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            EndDrawing();
            CenterDrawingInTexture();
            PredictSymbol();
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
        worker = ModelLoader.Load(_model).CreateWorker(WorkerFactory.Device.GPU);
        prediction = new Prediction();
    }

    private void StartDrawing()
    {
        lineRenderer.positionCount = 0;
        if (secondaryLineRenderer != null)
        {
            secondaryLineRenderer.positionCount = 0;
        }
    }


    private void DebugInputTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes("DebugInput.png", bytes);
        Debug.Log("Input texture saved for debugging: DebugInput.png");
    }

    private void AddPoint()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (lineRenderer.positionCount > 0)
        {
            Vector3 lastPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            mousePosition = Vector3.Lerp(lastPosition, mousePosition, 0.5f); // Smooth input
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePosition);

        if (secondaryLineRenderer != null)
        {
            secondaryLineRenderer.positionCount++;
            secondaryLineRenderer.SetPosition(secondaryLineRenderer.positionCount - 1, mousePosition);
        }
    }


    private void EndDrawing()
    {
        renderCamera.Render();
    }

    private void CenterDrawingInTexture()
    {
        Bounds bounds = new Bounds(lineRenderer.GetPosition(0), Vector3.zero);
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            bounds.Encapsulate(lineRenderer.GetPosition(i));
        }

        Vector3 center = bounds.center;
        renderCamera.transform.position = new Vector3(center.x, center.y, renderCamera.transform.position.z);

        float marginFactor = 1.2f;
        float maxDrawingSize = Mathf.Max(bounds.size.x, bounds.size.y) * marginFactor;
        renderCamera.orthographicSize = maxDrawingSize / 2f;


        float baseWidth = 0.1f;
        float zoomLevel = renderCamera.orthographicSize;
        float widthMultiplier = Mathf.Clamp(10f / zoomLevel, 5f, 9f);
        lineRenderer.widthMultiplier = baseWidth * widthMultiplier;

        if (secondaryLineRenderer != null)
        {
            secondaryLineRenderer.widthMultiplier = lineRenderer.widthMultiplier * 0.8f;
        }

        renderCamera.Render();
    }


    private void PredictSymbol()
    {
        Texture2D capturedTexture = new Texture2D(96, 96, TextureFormat.R8, false);
        RenderTexture.active = renderTexture;
        capturedTexture.ReadPixels(new Rect(0, 0, 96, 96), 0, 0);

        using var inputTensor = new Tensor(renderTexture, 1);
        worker.Execute(inputTensor);
        Tensor output = worker.PeekOutput();

        prediction.SetPrediction(output, _labels);

        MatchSymbolWithPoem(prediction.predictedLabel);

        output.Dispose();
        capturedTexture.Apply();

        DebugInputTexture(capturedTexture);
        Destroy(capturedTexture);
    }


    private void MatchSymbolWithPoem(string predictedSymbol)
    {
        string predictedLabel = prediction.predictedLabel;
        Debug.Log($"Predicted Symbol: {predictedLabel}");


        foreach (var enemy in FindObjectsOfType<Enemy>())
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


    private void UpdatePoemDisplay()
    {
        while (currentLetterIndex < poem.Length &&
               (poem[currentLetterIndex] == ' ' || !char.IsLetter(poem[currentLetterIndex])))
        {
            currentLetterIndex++;
        }

        if (currentLetterIndex < poem.Length)
        {
            string currentTargetLetter = poem[currentLetterIndex].ToString();
            currentLetterText.text = $"{currentTargetLetter}";
        }
        else
        {
            currentLetterText.text = "Poem completed!";
        }

        string before = poem.Substring(0, currentLetterIndex);
        string highlighted = currentLetterIndex < poem.Length
            ? $"<color=#000000><b>{poem[currentLetterIndex]}</b></color>"
            : "";
        string after = currentLetterIndex + 1 < poem.Length
            ? $"<color=#00000080>{poem.Substring(currentLetterIndex + 1)}</color>"
            : "";

        poemTextDisplay.text = before + highlighted + after;
    }

    private void InvertTextureColors(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = (byte)(255 - pixels[i].r);
            pixels[i].g = (byte)(255 - pixels[i].g);
            pixels[i].b = (byte)(255 - pixels[i].b);
        }

        texture.SetPixels32(pixels);
        texture.Apply();
    }

    void OnDestroy()
    {
        renderTexture?.Release();
        worker?.Dispose();
    }

    private void TriggerHeartAnimation(Vector3 targetPosition)
    {
        if (secondaryLineRenderer.positionCount <= 0) return;

        StartCoroutine(SpawnAndStreamParticles(targetPosition));
    }

    private IEnumerator SpawnAndStreamParticles(Vector3 targetPosition)
    {
        List<GameObject> heartParticles = new List<GameObject>();

        for (int i = 0; i < secondaryLineRenderer.positionCount; i++)
        {
            Vector3 pointPosition = secondaryLineRenderer.GetPosition(i);

            GameObject heartParticle = Instantiate(heartParticlePrefab, pointPosition, Quaternion.identity);
            heartParticles.Add(heartParticle);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject particle in heartParticles)
        {
            if (particle != null)
            {
                var particleMover = particle.AddComponent<ParticleMover>();
                particleMover.Initialize(targetPosition, 10f);
            }
        }

        secondaryLineRenderer.positionCount = 0;
    }

    public Vector2[] GetDrawnPoints()
    {
        int pointCount = lineRenderer.positionCount;
        if (pointCount < 3)
        {
            Debug.LogWarning("Not enough points drawn to form a wall.");
            return null;
        }

        Vector2[] points = new Vector2[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 pos = lineRenderer.GetPosition(i);
            points[i] = new Vector2(pos.x, pos.y);
        }

        // Close the shape if not already closed.
        if (points[0] != points[pointCount - 1])
        {
            Vector2[] closedPoints = new Vector2[pointCount + 1];
            System.Array.Copy(points, closedPoints, pointCount);
            closedPoints[pointCount] = points[0];
            points = closedPoints;
        }

        return points;
    }
}