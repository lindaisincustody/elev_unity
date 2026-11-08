using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SymbolCell : MonoBehaviour
{
    private static readonly Color Untried = new Color(0.17f, 0.18f, 0.22f);
    private static readonly Color Failing = new Color(0.55f, 0.16f, 0.16f);
    private static readonly Color Passing = new Color(0.16f, 0.48f, 0.24f);

    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private GameObject selection;
    [SerializeField] private TMP_Text glyph;
    [SerializeField] private TMP_Text stats;

    private readonly Dictionary<SymbolEngine, SymbolScore> scores = Enum.GetValues(typeof(SymbolEngine))
        .Cast<SymbolEngine>()
        .ToDictionary(engine => engine, engine => new SymbolScore());

    private SymbolEngine view;

    public int Index { get; private set; }

    public string Label => SymbolRecognizer.Labels[Index];
    public string Glyph => Glyphs.LatexToUnicode[Label];

    public void Init(int index, Action<SymbolCell> onClick)
    {
        Index = index;
        glyph.text = Glyph;
        button.onClick.AddListener(() => onClick(this));

        Clear();
        SetSelected(false);
    }

    public SymbolScore Score(SymbolEngine engine)
    {
        return scores[engine];
    }

    public void Record(SymbolPrediction prediction, TemplateMatch match, CombinedMatch combined)
    {
        scores[SymbolEngine.Model].Record(Index, prediction.Index, prediction.IsConfident);
        scores[SymbolEngine.Templates].Record(Index, match.Index, match.IsAccepted);
        scores[SymbolEngine.Combined].Record(Index, combined.Index, combined.IsAccepted);

        Refresh();
    }

    public void Clear()
    {
        foreach (SymbolScore score in scores.Values)
            score.Clear();

        Refresh();
    }

    public void Show(SymbolEngine engine)
    {
        view = engine;
        Refresh();
    }

    public void SetSelected(bool selected)
    {
        selection.SetActive(selected);
    }

    private void Refresh()
    {
        SymbolScore score = Score(view);

        stats.text = score.Attempts == 0 ? "" : $"{score.Hits}/{score.Attempts}";
        background.color = score.Attempts == 0 ? Untried : Color.Lerp(Failing, Passing, (float)score.Hits / score.Attempts);
    }
}
