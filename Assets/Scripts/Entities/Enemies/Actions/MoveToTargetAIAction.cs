using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/MoveToTargetAction")]
public class MoveToTargetAIAction : AIAction
{
    public override void Initialize(Context context)
    {
        context.sensor.targetTags.Add(targetTag);
    }

    public override void Execute(Context context)
    {
        var target = context.sensor.GetClosestTarget(targetTag);

        if (target == null) return;

        context.movement.target = target.position;
    }

    public override void Reset(Context context)
    {

    }
}
