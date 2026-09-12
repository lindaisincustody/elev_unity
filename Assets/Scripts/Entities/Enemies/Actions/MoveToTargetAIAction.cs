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

        if (context.sensor.CanSee(target))
        {
            context.movement.target = target.position;
        }
        else if (context.sensor.TryGetLastSeenPosition(targetTag, out Vector2 lastSeen))
        {
            context.movement.target = lastSeen;
        }
    }

    public override void Reset(Context context)
    {

    }

    public override void Stop(Context context)
    {
        context.movement.target = null;
    }
}
