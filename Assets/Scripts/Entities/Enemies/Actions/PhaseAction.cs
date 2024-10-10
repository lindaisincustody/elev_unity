using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/IdleAction")]
public class PhaseAction : AIAction
{
    public override void Execute(Context context)
    {
        context.brain.isActionBusy = true;
        context.brain.enemy.Get<Phase>().Execute();
    }

    public override void Reset(Context context)
    {
    }
}
