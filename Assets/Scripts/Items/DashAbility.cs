using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash Ability", menuName = "Custom/Ability/DashAbility")]
public class DashAbility : Ability
{
    private void OnEnable()
    {
        description = "Double Dash";
    }

    public override void Start()
    {
        Player.instance.Get<PlayerMovement>().maxDashCount = 2;
    }

    public override void Destroy()
    {
        Player.instance.Get<PlayerMovement>().maxDashCount = 1;
    }
}