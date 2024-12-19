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
        var animator = context.brain.enemy.Get<EnemyAnimator>();
        var lastAnim = animator.lastAnim;
        AttackRequest attackRequest = new AttackRequest(
            health, animator, 1, AttackType.Default, 
                () =>
                {
                    context.brain.isActionBusy = false;
                    animator.Play(lastAnim);
                }
            );

        context.brain.enemy.Get<EnemyAttack>().Attack(context, attackRequest);
    }

    public override void Reset(Context context)
    {
        context.brain.enemy.Get<EnemyAttack>().ResetAttack();
        //context.brain.isActionBusy = false;
    }

    public override void Stop(Context context)
    {
        context.brain.enemy.Get<EnemyAttack>().Stop();
    }
}
