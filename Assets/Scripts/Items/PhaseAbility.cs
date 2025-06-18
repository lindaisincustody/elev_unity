using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase Ability", menuName = "Custom/Ability/PhaseAbility")]
public class PhaseAbility : Ability
{
    public override void Activate()
    {
        Player.instance.ToggleGhostForm();
        Player.instance.Get<PlayerVisuals>().FadeOut();
    }

    public override void End()
    {
        Player.instance.ToggleGhostForm();
        Player.instance.Get<PlayerVisuals>().FadeIn();
    }
}
