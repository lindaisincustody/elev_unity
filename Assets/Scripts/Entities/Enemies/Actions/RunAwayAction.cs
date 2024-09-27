using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/RunawayAction")]
public class RunAwayAction : AIAction
{
    public override void Execute(Context context)
    {
        context.movement.target = null;
    }

    public override void Reset(Context context)
    {

    }
}
