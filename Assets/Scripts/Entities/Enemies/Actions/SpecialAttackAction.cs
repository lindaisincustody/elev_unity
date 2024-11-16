using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Actions/SpecialAttackAction")]
public class SpecialAttackAction : AIAction
{
    public override void Execute(Context context)
    {
        if (context.sensor.targetTags.Contains(targetTag))
            Attack(context);
    }

    private void Attack(Context context)
    {
        context.brain.isActionBusy = true;

        context.brain.enemy.Get<RatSpecialAttack>().Execute(context, () => context.brain.isActionBusy = false);
    }

    public override void Reset(Context context)
    {

    }
}
