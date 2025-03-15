using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyGlyphs", menuName = "Custom/Enemy/Glyphs")]
public class EnemyGlyphSO : ScriptableObject
{
    [SerializeField]
    private string[] labels = {
        "_Capricorn",
        "_Heart",
        "_Leo",
        "_Moon",
        "_Rightarrow",
        "_bowtie",
        "_clubsuit",
        "_descnode",
        "_diagup",
        "_diamond",
        "_downarrow",
        "_infty",
        "_ocircle",
        "_oplus",
        "_spadesuit",
        "_square",
        "_star",
        "_textgamma",
        "_textmusicalnote",
        "_varphi"
    };

    public string[] Labels => labels;
}

public static class Glyphs
{
    public static readonly Dictionary<string, string> LatexToUnicode = new Dictionary<string, string>
    {
        { "_Capricorn", "♑" },
        { "_Heart", "♥" },
        { "_Leo", "♌" },
        { "_Moon", "☾" },
        { "_Rightarrow", "⇒" },
        { "_bowtie", "⧓" },
        { "_clubsuit", "♣" },
        { "_descnode", "⤵" },
        { "_diagup", "/" },
        { "_diamond", "♦" },
        { "_downarrow", "↓" },
        { "_infty", "∞" },
        { "_ocircle", "⦾" },
        { "_oplus", "⊕" },
        { "_spadesuit", "♠" },
        { "_square", "■" },
        { "_star", "★" },
        { "_textgamma", "γ" },
        { "_textmusicalnote", "♪" },
        { "_varphi", "φ" }
    };
}
