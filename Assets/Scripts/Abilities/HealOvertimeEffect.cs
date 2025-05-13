using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOvertimeEffect : EntityEffect
{
    private int heal;
    private float interval;

    private Coroutine coroutine;

    public HealOvertimeEffect(Entity entity, int heal, float interval) : base(entity)
    {
        this.heal = heal;
        this.interval = interval;
        coroutine = CoroutineRunner.RunCoroutine(DamageOvertime());
    }

    public override void Dispose()
    {
        CoroutineRunner.StopRunningCoroutine(coroutine);
    }

    private IEnumerator DamageOvertime()
    {
        Health health = entity.Get<Health>();
        while (true)
        {
            health.Heal(heal); // Replace with actual damage logic
            yield return new WaitForSeconds(interval);
        }
    }
}
