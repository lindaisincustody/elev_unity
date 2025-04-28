using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOvertimeEffect : EntityEffect
{
    private int damage;
    private float damageInterval;

    private Coroutine damageCoroutine;

    public DamageOvertimeEffect(Entity entity, int damage, float damageInterval) : base(entity)
    {
        this.damage = damage;
        this.damageInterval = damageInterval;
        damageCoroutine = CoroutineRunner.RunCoroutine(DamageOvertime());
    }

    public override void Dispose()
    {
        CoroutineRunner.StopRunningCoroutine(damageCoroutine);
    }

    private IEnumerator DamageOvertime()
    {
        Health health = entity.Get<Health>();
        while (true)
        {
            health.TakeDamage(damage); // Replace with actual damage logic
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
