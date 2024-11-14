using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LetterDrawing : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;          // Primary line renderer
    [SerializeField] private LineRenderer secondaryLineRenderer; // Secondary overlay line renderer
    [SerializeField] private NNModel _model;
    [SerializeField] private RawImage renderTextureDisplay;
    [SerializeField] private bool invert = true;
    [SerializeField] private TextMesh textMesh;

    [SerializeField] private TextMeshProUGUI poemTextDisplay;  // UI Text for displaying poem in the book
    [SerializeField, TextArea] private string poem = "Your poem text goes here."; // The full poem text
    private int currentLetterIndex = 0; // Tracks current letter in poem to match
    [SerializeField] private TextMeshProUGUI currentLetterText; // UI Text to display the current letter

    private bool isComplete = false;

    private Camera renderCamera;
    private RenderTexture renderTexture;
    private IWorker worker;

    private Coroutine timeScaleCoroutine;

    private readonly string[] _labels = {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
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
            predictedLabel = labels[predictedIndex];
            Debug.Log($"Predicted Character: {predictedLabel}");
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

        // Assign a trippy shader material to the primary line
        Material trippyMaterial = new Material(Shader.Find("Unlit/Color"));
        Material trippyMaterialSecondary = new Material(Shader.Find("Custom/TrippyTransparent"));
        trippyMaterial.SetColor("_Color", Color.red);
        trippyMaterial.SetFloat("_Transparency", 0.5f);
        trippyMaterial.SetFloat("_TimeSpeed", 1.0f);
        trippyMaterialSecondary.SetColor("_Color", Color.black);
        trippyMaterialSecondary.SetFloat("_Transparency", 0.2f);
        trippyMaterialSecondary.SetFloat("_TimeSpeed", 1.0f);
        lineRenderer.material = trippyMaterial;
        secondaryLineRenderer.material = trippyMaterialSecondary;

        if (renderTextureDisplay != null)
        {
            renderTextureDisplay.texture = renderTexture;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0))
        {
            AddPoint();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrawing();
            CenterDrawingInTexture();
            PredictNumber();
        }
    }

    private void InitializeCamera()
    {
        renderTexture = new RenderTexture(28, 28, 16, RenderTextureFormat.R8);
        renderCamera = new GameObject("Render Camera").AddComponent<Camera>();

        renderCamera.orthographic = true;
        renderCamera.cullingMask = LayerMask.GetMask("Drawing");
        renderCamera.backgroundColor = Color.black;
        renderCamera.clearFlags = CameraClearFlags.Color;
        renderCamera.targetTexture = renderTexture;

        renderCamera.orthographicSize = Camera.main.orthographicSize * 0.7f;
        renderCamera.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
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

        // Start smooth transition to slow-motion
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(SmoothTimeScaleTransition(0.1f, 0.5f)); // Slow down over 0.5 seconds
    }

    private void AddPoint()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
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

        // Start smooth transition back to normal speed
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(SmoothTimeScaleTransition(1.0f, 0.5f)); // Speed up over 0.5 seconds
    }

    private IEnumerator SmoothTimeScaleTransition(float targetTimeScale, float duration)
    {
        float startScale = Time.timeScale;
        float startFixedDeltaTime = Time.fixedDeltaTime;
        float targetFixedDeltaTime = 0.02f * targetTimeScale; // 0.02 is Unity's default fixed delta time
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, t);
            Time.fixedDeltaTime = Mathf.Lerp(startFixedDeltaTime, targetFixedDeltaTime, t);
            time += Time.unscaledDeltaTime; // Use unscaledDeltaTime to keep smooth transition
            yield return null;
        }

        Time.timeScale = targetTimeScale;
        Time.fixedDeltaTime = targetFixedDeltaTime;
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

        renderCamera.Render();
    }

    private string NormalizePrediction(string predictedLabel)
    {
        if (predictedLabel == "0" || predictedLabel.ToUpper() == "O") return "O";

        else if (predictedLabel == "1" || predictedLabel.ToUpper() == "I" || predictedLabel.ToUpper() == "L")
        {
            return "I";
        }
        return predictedLabel.ToUpper();
    }

    private void PredictNumber()
    {
        Texture2D capturedTexture = new Texture2D(28, 28, TextureFormat.R8, false);
        RenderTexture.active = renderTexture;
        capturedTexture.ReadPixels(new Rect(0, 0, 28, 28), 0, 0);
        if (invert)
        {
            InvertTextureColors(capturedTexture);
        }

        using var inputTensor = new Tensor(renderTexture, 1);
        worker.Execute(inputTensor);
        Tensor output = worker.PeekOutput();

        prediction.SetPrediction(output, _labels);
        string predictedLetter = NormalizePrediction(prediction.predictedLabel);

        // Skip non-letter characters in the poem
        while (!isComplete && (poem[currentLetterIndex] == ' ' || !char.IsLetter(poem[currentLetterIndex])))
        {
            currentLetterIndex++;
            UpdatePoemDisplay();
        }

        // Check if the normalized predicted letter matches the current letter in the poem
        string currentPoemLetter = NormalizePrediction(poem[currentLetterIndex].ToString());
        if (!isComplete && predictedLetter == currentPoemLetter)
        {
            currentLetterIndex++;
            UpdatePoemDisplay();

            CorrectLetterUI.Instance.Show(currentPoemLetter);

            TriggerHomingBullet();

            if (currentLetterIndex >= poem.Length)
            {
                isComplete = true;
                Debug.Log("Poem completed!");
            }
        }
        else {
            CorrectLetterUI.Instance.ShowWrong(currentPoemLetter);
        }

        output.Dispose();
        capturedTexture.Apply();
        Destroy(capturedTexture);
    }

    private void UpdatePoemDisplay()
    {
        // Skip non-letter characters and spaces
        while (currentLetterIndex < poem.Length && (poem[currentLetterIndex] == ' ' || !char.IsLetter(poem[currentLetterIndex])))
        {
            currentLetterIndex++;
        }

        // Display the current target letter to draw
        if (currentLetterIndex < poem.Length)
        {
            string currentTargetLetter = poem[currentLetterIndex].ToString();
            currentLetterText.text = $"{currentTargetLetter}";
        }
        else
        {
            currentLetterText.text = "Poem completed!";
        }

        // Get the parts of the poem string for highlighting
        string before = poem.Substring(0, currentLetterIndex);
        string highlighted = currentLetterIndex < poem.Length ? $"<color=#000000><b>{poem[currentLetterIndex]}</b></color>" : "";

        // Make the 'after' text more transparent
        string after = currentLetterIndex + 1 < poem.Length
            ? $"<color=#00000080>{poem.Substring(currentLetterIndex + 1)}</color>"  // 80 is ~50% opacity in hex
            : "";

        // Update the poem text display
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
        Time.timeScale = 1.0f; // Ensure normal speed when script is destroyed
    }

    private void TriggerHomingBullet()
    {
        PlayerCombat playerCombat = GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.ShootHomingBullet();
        }
    }

}
