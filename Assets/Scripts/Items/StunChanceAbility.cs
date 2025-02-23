using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun Chance Ability", menuName = "Custom/Ability/StunChanceAbility")]
public class StunChanceAbility : Ability
{
    [SerializeField] private float stunGlyphChance = 0.1f;

    public override void Start()
    {
        Player.instance.SpecialSymbolChance += stunGlyphChance;
    }

    public override void Destroy()
    {
        Player.instance.SpecialSymbolChance -= stunGlyphChance;
    }
}
