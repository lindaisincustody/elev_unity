using System;
using System.Linq;
using UnityEngine;

public readonly struct CombinedMatch
{
    public const float Threshold = 0.8f;
    public const float MaxDistance = 1.4f;

    private const float ModelWeight = 0.3f;
    private const float Temperature = 0.1f;
    private const float Floor = 1e-12f;

    public float[] Probabilities { get; }
    public int Index { get; }
    public float Distance { get; }

    public string Label => SymbolRecognizer.Labels[Index];
    public float Probability => Probabilities[Index];
    public bool IsAccepted => Probability >= Threshold && Distance <= MaxDistance;

    public CombinedMatch(SymbolPrediction prediction, TemplateMatch match)
    {
        float nearest = match.Distances.Min();
        float[] closeness = match.Distances.Select(distance => Mathf.Exp((nearest - distance) / Temperature)).ToArray();
        float total = closeness.Sum();

        float[] scores = new float[closeness.Length];
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = float.IsNegativeInfinity(prediction.Scores[i])
                ? float.NegativeInfinity
                : ModelWeight * Mathf.Log(prediction.Probabilities[i] + Floor) + (1f - ModelWeight) * Mathf.Log(closeness[i] / total + Floor);
        }

        float max = scores.Max();
        float[] exp = scores.Select(score => Mathf.Exp(score - max)).ToArray();
        float sum = exp.Sum();

        Probabilities = exp.Select(value => value / sum).ToArray();
        Index = Array.IndexOf(Probabilities, Probabilities.Max());
        Distance = match.Distances[Index];
    }
}
