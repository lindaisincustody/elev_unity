using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Fire Ability", menuName = "Custom/Ability/FireAbility")]
public class FireAbility : Ability
{
    [SerializeField] private float duration;
    [SerializeField] private float damageInterval;
    [SerializeField] private int damage;
    [SerializeField] private Material material;

    public override void Activate()
    {
        base.Activate();
        LetterDrawing letterDrawing= Player.instance.Get<LetterDrawing>();

        letterDrawing.ActivateFireState(duration, damageInterval, damage, material);
    }
}
