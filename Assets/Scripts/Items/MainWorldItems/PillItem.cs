using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Pill", menuName = "Custom/WorldItems/Pill")]
public class Pill : Item
{
    public override void OnGameStart()
    {
        // No Effwct On Start
        return;
    }

    public override void UseItem()
    {
        // TODO: Remove FondObjectOfType. Maybe PillEffect should be non monobeheavior
        FindObjectOfType<PillEfect>().StartEffect();
    }
}
