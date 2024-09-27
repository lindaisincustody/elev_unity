using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/AttackAction")]
public class AttackAction : AIAction
{
    public override void Execute(Context context)
    {
        if (context.sensor.targetTags.Contains(targetTag))
            Attack(context);
    }

    private void Attack(Context context)
    {
        context.brain.isActionBusy = true;
        var health = context.target.GetComponent<Health>();
        var animator = context.brain.animator;
        var lastAnim = animator.lastAnim;
        context.brain.attack.Attack(health, animator, () =>
            {
                context.brain.isActionBusy = false;
                animator.PlayAnimation(lastAnim);
            }
        );
    }

    public override void Reset(Context context)
    {
        context.brain.attack.ResetAttack();
        //context.brain.isActionBusy = false;
    }
}
