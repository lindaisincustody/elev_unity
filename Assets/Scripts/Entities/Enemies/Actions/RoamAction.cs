using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/RoamAction")]
public class RoamAction : AIAction
{
    private float roamCooldown = 3f;

    public override void Execute(Context context)
    {
        if (context.brain.movement.passedTime >= roamCooldown)
        {
            GetNewPosition(context);
            context.brain.movement.passedTime = 0f; 
        }

        context.brain.movement.passedTime += Time.deltaTime;
    }

    private void GetNewPosition(Context context)
    {
        Vector2 newPosition;

        newPosition = new Vector2(
                       Random.Range(context.movement.minBound.x, context.movement.maxBound.x),
                       Random.Range(context.movement.minBound.y, context.movement.maxBound.y));

       context.movement.target = newPosition;
       context.brain.animator.PlayAnimation(EnemyAnimator.AnimationType.Walk);
    }

    public override void Reset(Context context)
    {

    }
}
