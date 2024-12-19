using System;
using System.Collections;
using UnityEngine;

public class Phase : Component
{
    [SerializeField] private float phaseDuration = 3f;
    [SerializeField] private float phaseCooldown = 4f;

    private float currentCooldownTime = 5f;
    private Context context;
    private Coroutine dodgeCoroutine;
    private Coroutine cooldownCoroutine;

    public void Execute(Context context, Action onEnd)
    {
        if (currentCooldownTime < phaseCooldown)
        {
            onEnd?.Invoke();
            return;
        }

        this.context = context;
        currentCooldownTime = 0f;

        if (dodgeCoroutine != null)
            StopCoroutine(dodgeCoroutine);
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        StartCoroutine(PhaseRoutine(onEnd));
        dodgeCoroutine = StartCoroutine(DodgeRoutine());
    }

    public void Stop()
    {
        currentCooldownTime = 0f;

        if (dodgeCoroutine != null)
            StopCoroutine(dodgeCoroutine);
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        var enemyHealth = context.brain.enemy.Get<EnemyHealth>();
        enemyHealth.SetAlpha(1f);
        enemyHealth.Immune = false;
    }

    private IEnumerator PhaseRoutine(Action onEnd)
    {
        var enemyHealth = context.brain.enemy.Get<EnemyHealth>();
        enemyHealth.Immune = true;
        enemyHealth.SetAlpha(0.1f);

        yield return new WaitForSeconds(phaseDuration);

        var attackRequest = new AttackRequest(
            context.target.GetComponent<Health>(),
            context.brain.enemy.Get<EnemyAnimator>(),
            1,
            AttackType.Special,
            onEnd
        );
        context.brain.enemy.Get<EnemyAttack>().Attack(context, attackRequest);

        if (dodgeCoroutine != null)
        {
            StopCoroutine(dodgeCoroutine);
            dodgeCoroutine = null;
        }

        enemyHealth.SetAlpha(1f);
        enemyHealth.Immune = false;
    }

    private IEnumerator DodgeRoutine()
    {
        while (true)
        {
            context.brain.enemy.Get<EnemyMovement>().Avoid(context.target);
            yield return null;
        }
    }

    private IEnumerator CooldownRoutine()
    {
        while (currentCooldownTime < phaseCooldown)
        {
            currentCooldownTime += Time.deltaTime;
            yield return null;
        }

        cooldownCoroutine = null;
    }
}
