using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredictionRow : MonoBehaviour
{
    private static readonly Color Normal = new Color(0.17f, 0.18f, 0.22f);
    private static readonly Color Target = new Color(0.16f, 0.36f, 0.5f);

    [SerializeField] private Image background;
    [SerializeField] private RectTransform bar;
    [SerializeField] private TMP_Text glyph;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text value;

    public void Show(int index, float fill, string text, bool isTarget)
    {
        string name = SymbolRecognizer.Labels[index];

        glyph.text = Glyphs.LatexToUnicode[name];
        label.text = name;
        value.text = text;
        bar.anchorMax = new Vector2(Mathf.Clamp01(fill), 1f);
        background.color = isTarget ? Target : Normal;
    }

    public void Hide()
    {
        glyph.text = "";
        label.text = "";
        value.text = "";
        bar.anchorMax = new Vector2(0f, 1f);
        background.color = Normal;
    }
}
