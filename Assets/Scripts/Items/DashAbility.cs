using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Dash Ability", menuName = "Custom/Ability/DashAbility")]
public class DashAbility : Ability
{
    public override void Start()
    {
        Debug.Log("Dash start");
        Player.instance.PlayerMovement.maxDashCount = 2;
    }

    public override void Destroy()
    {
        Player.instance.PlayerMovement.maxDashCount = 1;
    }
}
