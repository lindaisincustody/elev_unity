using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GlyphBook : MonoBehaviour
{
    public static GlyphBook instance;

    private Dictionary<Enemy, GlyphText> keyValuePairs = new Dictionary<Enemy, GlyphText>();

    [SerializeField] private GlyphText glyphPrefab;
    [SerializeField] private Transform glyphPanel;
    [SerializeField] private GameObject glyphBook;

    private void Awake()
    {
        instance = this;
    }

    public void ActivateBook()
    {
        glyphBook.SetActive(true);
    }

    public void AddEnemy(Enemy enemy)
    {
        GlyphText newGlyphs = Instantiate(glyphPrefab, glyphPanel);
        newGlyphs.SetText(string.Join("", enemy.activeSymbols));
        keyValuePairs[enemy] = newGlyphs;
    }

    public void GlyphWritten(Enemy enemy, TMP_Text enemyGlyph)
    {
        if (keyValuePairs.TryGetValue(enemy, out GlyphText glyphText))
        {
            // The key exists, and glyphText contains the associated value
            Debug.Log("Found GlyphText: " + glyphText);
        }
        else
        {
            // The key does not exist
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
        }
        );
    }
}
