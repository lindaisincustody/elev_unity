using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class GlyphBook : MonoBehaviour
{
    public static GlyphBook instance;

    private Dictionary<Enemy, GlyphText> keyValuePairs = new Dictionary<Enemy, GlyphText>();

    [SerializeField] private GlyphText glyphPrefab;
    [SerializeField] private EnglishText englishTextPrefab;
    [SerializeField] private Transform glyphPanel;
    [SerializeField] private Transform CompleteTextPanel;
    [SerializeField] private GameObject glyphBook;
    [SerializeField] private TMP_Text poemText;

    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private List<Vector3Int> wallTilePositionsToRemove;

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
        newGlpyh.transform.DOMove(glyph.transform.position, 3).SetEase(Ease.InSine)
            .SetSpeedBased(true)
            .OnComplete(() =>
            {
                glyph.Text.DOColor(Color.red, 2f);
                Destroy(newGlpyh.gameObject);
            });
    }


    public void TranslateGlyphs()
    {
        List<string> allGlyphs = new List<string>();
        int glyphCount = keyValuePairs.Count;
        int completedFades = 0;

        foreach (var entry in keyValuePairs)
        {
            GlyphText glyphText = entry.Value;
            if (glyphText != null)
            {
                foreach (var pair in glyphText.activeGlyphs)
                {
                    allGlyphs.Add(pair.Value);
                }
            }

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
        EnglishText newGlyphText = Instantiate(englishTextPrefab, CompleteTextPanel);
        newGlyphText.WriteText("Poem Complete", () =>
        {
            HideBook();
            RemoveWallTiles();

            AbilitySelectionUI abilityUI = FindObjectOfType<AbilitySelectionUI>();
            if (abilityUI != null)
            {
                abilityUI.Show();
            }
        });
    }


    private void RemoveWallTiles()
    {
        if (wallTilemap == null)
        {
            Debug.LogWarning("Wall Tilemap is not assigned!");
            return;
        }

        foreach (Vector3Int pos in wallTilePositionsToRemove)
        {
            wallTilemap.SetTile(pos, null);
        }
    }

    private void HideBook()
    {
        glyphBook.transform.DOScale(Vector3.zero, bookScaleTime)
            .OnComplete(() => glyphBook.SetActive(false))
            .SetDelay(2f);
    }

    public void ActivateBookForTesting()
    {
        ActivateBook();
    }
}