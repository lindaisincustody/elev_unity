using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun Chance Ability", menuName = "Custom/Ability/StunChanceAbility")]
public class StunChanceAbility : Ability
{
    [SerializeField] private float stunGlyphChance = 0.1f;

    private void OnEnable()
    {
        description = "+10% chance to stun an enemy with any glyph drawn ";
    }

    public override void Start()
    {
        Player.instance.SpecialSymbolChance += stunGlyphChance;
    }

    public override void Destroy()
    {
        Player.instance.SpecialSymbolChance -= stunGlyphChance;
    }
}