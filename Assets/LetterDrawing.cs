using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class LetterDrawing : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private NNModel _model;
    [SerializeField] private RawImage renderTextureDisplay;
    [SerializeField] private bool invert = true;
    [SerializeField] private TextMesh textMesh;

    private Camera renderCamera;
    private RenderTexture renderTexture;
    private IWorker worker;

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

        lineRenderer.gameObject.layer = LayerMask.NameToLayer("Drawing");
        renderCamera.cullingMask = LayerMask.GetMask("Drawing");

        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;

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
        renderTexture = new RenderTexture(28, 28, 16, RenderTextureFormat.R8); // Directly capture at 28x28
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
    }

    private void AddPoint()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePosition);
    }

    private void EndDrawing()
    {
        renderCamera.Render();
    }

    private void CenterDrawingInTexture()
    {
        // Calculate the bounds of the drawing
        Bounds bounds = new Bounds(lineRenderer.GetPosition(0), Vector3.zero);
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            bounds.Encapsulate(lineRenderer.GetPosition(i));
        }

        // Center the camera on the bounds
        Vector3 center = bounds.center;
        renderCamera.transform.position = new Vector3(center.x, center.y, renderCamera.transform.position.z);

        // Adjust orthographic size to fit the bounds within the render texture with a small margin
        float marginFactor = 1.2f; // Add a margin (20%)
        float maxDrawingSize = Mathf.Max(bounds.size.x, bounds.size.y) * marginFactor;
        renderCamera.orthographicSize = maxDrawingSize / 2f;

        // Render the camera after centering and resizing
        renderCamera.Render();
    }


    private void SaveTexture(Texture2D texture, string filename)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/" + filename + ".png", bytes);
        Debug.Log("Saved texture to " + filename);
    }

    private void PredictNumber()
    {
        Texture2D capturedTexture = new Texture2D(28, 28, TextureFormat.R8, false);
        RenderTexture.active = renderTexture;
        capturedTexture.ReadPixels(new Rect(0, 0, 28, 28), 0, 0);
        if (invert)
        {
            InvertTextureColors(capturedTexture);
            Debug.Log("Inverted texture colors for prediction.");
        }
        SaveTexture(capturedTexture, "debugImage");  // Uncomment if you need to visually verify
        Debug.Log("Saved debug image for verification.");


        // Directly create a tensor from the RenderTexture
        using var inputTensor = new Tensor(renderTexture, 1); // "1" is for grayscale

        // Run the model
        worker.Execute(inputTensor);
        Tensor output = worker.PeekOutput();

        // Verify output length matches labels length
        if (output.length != _labels.Length)
        {
            Debug.LogError($"Mismatch between model output length ({output.length}) and labels count ({_labels.Length})");
            return;
        }

        // Log output values to inspect which predictions are being made
        Debug.Log("Model output values:");
        for (int i = 0; i < output.length; i++)
        {
            Debug.Log($"{_labels[i]}: {output[0, 0, 0, i]:0.000}");
        }

        // Interpret prediction and display result
        prediction.SetPrediction(output, _labels);
        if (textMesh != null)
        {
            textMesh.text = prediction.predictedLabel;
        }

        // Cleanup
        output.Dispose();
    }

    private void ApplyThreshold(Texture2D texture, float threshold = 0.5f)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            float brightness = (pixels[i].r + pixels[i].g + pixels[i].b) / 3;
            pixels[i] = brightness > threshold ? Color.white : Color.black;
        }
        texture.SetPixels(pixels);
        texture.Apply();
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
}
