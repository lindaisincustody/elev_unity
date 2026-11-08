using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class SymbolRecognizer : IDisposable
{
    public const float Threshold = 0.8f;

    public static readonly string[] Labels =
    {
        "_Aries", "_Capricorn", "_Cross", "_EyesDollar", "_Heart", "_Leo", "_Mercury",
        "_Moon", "_Rightarrow", "_Sigma", "_Taurus", "_alpha", "_bigtriangleup",
        "_bowtie", "_boxplus", "_circlearrowleft", "_clubsuit", "_diagup",
        "_diamondsuit", "_downarrow", "_emptyset", "_female", "_infty", "_lambda",
        "_lightning", "_ltimes", "_male", "_psi", "_sim", "_spadesuit", "_square",
        "_star", "_textasteriskcentered", "_textcent", "_textgamma",
        "_textmusicalnote", "_theta", "_varphi"
    };

    public static readonly string[] Active = Labels.Where(Glyphs.LatexToUnicode.ContainsKey).ToArray();

    private static readonly int[] Retired = Enumerable.Range(0, Labels.Length)
        .Where(index => !Active.Contains(Labels[index]))
        .ToArray();

    private const int InputSize = 96;
    private const float StrokePixels = 3f;
    private const float Margin = 1.2f;

    private readonly Camera camera;
    private readonly IWorker worker;

    public RenderTexture Input { get; }

    public SymbolRecognizer(NNModel model, Camera camera)
    {
        this.camera = camera;

        Input = new RenderTexture(InputSize, InputSize, 16, RenderTextureFormat.ARGB32);
        Input.Create();
        camera.targetTexture = Input;

        worker = WorkerFactory.CreateWorker(ModelLoader.Load(model), WorkerFactory.Device.CPU);

        camera.Render();
        Infer();
    }

    public SymbolPrediction Predict(LineRenderer stroke, LineRenderer overlay)
    {
        Frame(stroke);

        float width = StrokePixels * camera.orthographicSize * 2f / InputSize;
        stroke.widthMultiplier = 1f;
        stroke.startWidth = width;
        stroke.endWidth = width;

        stroke.enabled = true;
        overlay.enabled = false;
        camera.Render();
        stroke.enabled = false;
        overlay.enabled = true;

        return new SymbolPrediction(Infer());
    }

    public void Dispose()
    {
        worker.Dispose();
        Input.Release();
    }

    private void Frame(LineRenderer stroke)
    {
        Bounds bounds = new Bounds(stroke.GetPosition(0), Vector3.zero);
        for (int i = 1; i < stroke.positionCount; i++)
            bounds.Encapsulate(stroke.GetPosition(i));

        float size = Mathf.Max(bounds.size.x, bounds.size.y) * Margin;

        camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, camera.transform.position.z);
        camera.orthographicSize = Mathf.Max(size / 2f, 0.01f);
    }

    private float[] Infer()
    {
        using Tensor input = new Tensor(Input, 1);
        worker.Execute(input);

        float[] scores = worker.PeekOutput().ToReadOnlyArray().ToArray();
        foreach (int index in Retired)
            scores[index] = float.NegativeInfinity;

        return scores;
    }
}

public readonly struct SymbolPrediction
{
    public float[] Scores { get; }
    public float[] Probabilities { get; }
    public int Index { get; }

    public string Label => SymbolRecognizer.Labels[Index];
    public float Probability => Probabilities[Index];
    public bool IsConfident => Probability >= SymbolRecognizer.Threshold;

    public SymbolPrediction(float[] scores)
    {
        Scores = scores;
        Index = Array.IndexOf(scores, scores.Max());

        float max = scores[Index];
        float[] exp = scores.Select(score => Mathf.Exp(score - max)).ToArray();
        float sum = exp.Sum();
        Probabilities = exp.Select(value => value / sum).ToArray();
    }
}
