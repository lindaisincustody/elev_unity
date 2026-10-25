using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase Ability", menuName = "Custom/Ability/PhaseAbility")]
public class PhaseAbility : Ability
{
    public override void Activate()
    {
        Player.instance.Get<PlayerGhost>().Toggle();
        Player.instance.Get<PlayerVisuals>().FadeOut();
    }

    public override void End()
    {
        Player.instance.Get<PlayerGhost>().Toggle();
        Player.instance.Get<PlayerVisuals>().FadeIn();
    }
}
