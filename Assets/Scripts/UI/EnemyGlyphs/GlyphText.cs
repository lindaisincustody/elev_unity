using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlyphText : MonoBehaviour
{
    [SerializeField] public Glyph glyphText;

    private Dictionary<Glyph, string> activeGlyphs = new();

    public void SetText(string text)
    {
        foreach (var glyph in text)
        {
            Glyph newGlyph = Instantiate(glyphText, transform);
            activeGlyphs[newGlyph] = glyph.ToString();
            newGlyph.Text.text = glyph.ToString();
        }
    }

    public Glyph GetGlyph(string text)
    {
        foreach (var pair in activeGlyphs)
        {
            if (pair.Value == text)
            {
                return pair.Key; // Return the Glyph if the text matches
            }
        }
        return null; // Return null if no match is found
    }
}