using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/PhaseAction")]
public class PhaseAction : AIAction
{
    public override void Execute(Context context)
    {
        context.brain.isActionBusy = true;
        context.brain.enemy.Get<Phase>().Execute(context, () => context.brain.isActionBusy = false);
    }

    public override void Reset(Context context)
    {
    }

    public override void Stop(Context context)
    {
        context.brain.enemy.Get<Phase>().Stop();
    }
}
