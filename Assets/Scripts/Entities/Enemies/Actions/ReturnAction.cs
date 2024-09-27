using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/ReturnAction")]
public class ReturnAction : AIAction
{
    public override void Execute(Context context)
    {
        context.movement.target = context.movement.spawnPos;
    }

    public override void Reset(Context context)
    {

    }
}
