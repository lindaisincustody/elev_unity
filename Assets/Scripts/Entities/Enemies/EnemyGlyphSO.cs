using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyGlyphs", menuName = "Custom/Enemy/Glyphs")]
public class EnemyGlyphSO : ScriptableObject
{
    [SerializeField]
    private string[] labels = {
        "_Aries", "_Capricorn", "_Cross", "_EyesDollar", "_Heart", "_Leo", "_Mercury",
    "_Moon", "_Rightarrow", "_Sigma", "_Taurus", "_alpha", "_bigtriangleup",
    "_bowtie", "_boxplus", "_circlearrowleft", "_clubsuit", "_diagup",
    "_diamondsuit", "_downarrow", "_emptyset", "_female", "_infty", "_lambda",
    "_lightning", "_ltimes", "_male", "_psi", "_sim", "_spadesuit", "_square",
    "_star", "_textasteriskcentered", "_textcent", "_textgamma",
    "_textmusicalnote", "_theta", "_varphi"
    };

    public string[] Labels => labels;
}

public static class Glyphs
{
    public static readonly Dictionary<string, string> LatexToUnicode = new Dictionary<string, string>
    {
        { "_Aries", "♈" },
    { "_Capricorn", "♑" },
    { "_Cross", "†" },
    { "_EyesDollar", "🤑" },
    { "_Heart", "♥" },
    { "_Leo", "♌" },
    { "_Mercury", "☿" },
    { "_Moon", "☾" },
    { "_Rightarrow", "⇒" },
    { "_Sigma", "Σ" },
    { "_Taurus", "♉" },
    { "_alpha", "α" },
    { "_bigtriangleup", "△" },
    { "_bowtie", "⧓" },
    { "_boxplus", "⊞" },
    { "_circlearrowleft", "↺" },
    { "_clubsuit", "♣" },
    { "_diagup", "/" },
    { "_diamondsuit", "♦" },
    { "_downarrow", "↓" },
    { "_emptyset", "∅" },
    { "_female", "♀" },
    { "_infty", "∞" },
    { "_lambda", "λ" },
    { "_lightning", "⚡" },
    { "_ltimes", "⋉" },
    { "_male", "♂" },
    { "_psi", "ψ" },
    { "_sim", "∼" },
    { "_spadesuit", "♠" },
    { "_square", "■" },
    { "_star", "★" },
    { "_textasteriskcentered", "∗" },
    { "_textcent", "¢" },
    { "_textgamma", "γ" },
    { "_textmusicalnote", "♪" },
    { "_theta", "θ" },
    { "_varphi", "φ" }
    };
}
