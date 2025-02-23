using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class GlyphBook : MonoBehaviour
{
    public static GlyphBook instance;

    private Dictionary<Enemy, GlyphText> keyValuePairs = new Dictionary<Enemy, GlyphText>();

    [SerializeField] private GlyphText glyphPrefab;
    [SerializeField] private EnglishText englishTextPrefab;
    [SerializeField] private Transform glyphPanel;
    [SerializeField] private GameObject glyphBook;
    [SerializeField] private TMP_Text poemText;

    private List<string> poemWords = new List<string>
    {
        "The shadows fade,", "the light unfolds,",
        "A journey's end,", "a story told.",
        "Through night and day,", "we seek our way,",
        "With every step,", "the price we pay."
    };

    private int currentWordIndex = 0;

    private float bookScaleTime = 0.6f;

    private void Awake()
    {
        instance = this;
    }

    public void ActivateBook()
    {
        glyphBook.transform.localScale = Vector3.zero;
        glyphBook.SetActive(true);
        glyphBook.transform.DOScale(Vector3.one, bookScaleTime).SetEase(Ease.OutBack);
    }

    public void AddEnemy(Enemy enemy)
    {
        GlyphText newGlyphs = Instantiate(glyphPrefab, glyphPanel);

        newGlyphs.SetText(string.Join("", enemy.activeSymbols.Select(g => g.Glyph)));

        keyValuePairs[enemy] = newGlyphs;
    }

    public void GlyphWritten(Enemy enemy, TMP_Text enemyGlyph)
    {
        if (keyValuePairs.TryGetValue(enemy, out GlyphText glyphText))
        {
            Debug.Log("Found GlyphText: " + glyphText);
        }
        else
        {
            Debug.LogError("Enemy not found in dictionary.");
            return;
        }

        Glyph glyph = glyphText.GetGlyph(enemyGlyph.text);
        Glyph newGlyphText = Instantiate(glyphText.glyphText, glyph.transform);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemyGlyph.transform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            glyph.transform.GetComponent<RectTransform>(),
            screenPosition,
            glyph.transform.GetComponentInParent<Canvas>().worldCamera,
            out Vector2 localPoint
        );

        newGlyphText.GetComponent<RectTransform>().anchoredPosition = localPoint;

        newGlyphText.Text.text = enemyGlyph.text;
        MoveToBook(newGlyphText, glyph);
    }

    private void MoveToBook(Glyph newGlpyh, Glyph glyph)
    {
        newGlpyh.transform.DOMove(glyph.transform.position, 3).SetEase(Ease.InSine).SetSpeedBased(true).OnComplete(() =>
        {
            glyph.Text.DOColor(Color.red, 2f);
            Destroy(newGlpyh.gameObject);
            UnlockPoem();
        });
    }

    private void UnlockPoem()
    {
        if (currentWordIndex < poemWords.Count)
        {
            poemText.text += " " + poemWords[currentWordIndex];
            currentWordIndex += 1;
        }
    }

    public void TranslateGlyphs()
    {
        List<string> allGlyphs = new List<string>();
        int glyphCount = keyValuePairs.Count; // Total glyphs that need to fade out
        int completedFades = 0; // Track completed fade-outs

        foreach (var entry in keyValuePairs)
        {
            GlyphText glyphText = entry.Value;
            if (glyphText != null)
            {
                foreach (var pair in glyphText.activeGlyphs) // Accessing stored glyphs
                {
                    allGlyphs.Add(pair.Value); // Collect the text representation
                }
            }

            // Fade out each glyph and check if all are done
            glyphText.FadeOutText(() =>
            {
                completedFades++;
                if (completedFades >= glyphCount)
                {
                    InterprateText();
                }

                Destroy(glyphText.gameObject);
            });
        }

        string translatedText = string.Join(" ", allGlyphs);
    }

    private void InterprateText()
    {
        EnglishText newGlyphText = Instantiate(englishTextPrefab, glyphPanel);
        newGlyphText.WriteText("This a test text. All glyphs have faded out. Now interpreting the text..", () => HideBook());
    }

    private void HideBook()
    {
        glyphBook.transform.DOScale(Vector3.zero, bookScaleTime).OnComplete(() => glyphBook.SetActive(false)).SetDelay(2f);
    }
}
