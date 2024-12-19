using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "UtilityAI/Actions/DashToTargetAction")]
public class DashToTarget : AIAction
{
    public override void Initialize(Context context)
    {
        context.sensor.targetTags.Add(targetTag);
    }

    public override void Execute(Context context)
    {
        var target = context.sensor.GetClosestTarget(targetTag);

        if (target == null) return;

        context.movement.Dash(context, 10, null);
    }

    public override void Reset(Context context)
    {

    }

    public override void Stop(Context context)
    {
        context.movement.Stop();
    }
}
