using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGlyph
{
    public string Glyph;

    private System.Action SpecialAction;

    public EnemyGlyph(string newGlyph)
    {
        Glyph = newGlyph;
    }

    public void SetSpecial(System.Action action)
    {
        SpecialAction = action;
    }

    public void OnDrawn()
    {
        SpecialAction?.Invoke();
    }
}
