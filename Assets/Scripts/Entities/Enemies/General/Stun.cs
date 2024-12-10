using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : Component
{
    [SerializeField] private Vector3 offset;
    private float stunDuration = 2f;

    private Coroutine coroutine;

    private Brain brain;

    private void Awake()
    {
        brain = GetComponent<Brain>();

        brain.enemy.Get<EnemyHealth>().OnDamage += Execute;
    }

    private void Execute()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(DisableEnemy(stunDuration));
    }

    private IEnumerator DisableEnemy(float time)
    {
        var effect = EffectSystem.GetEffect(EffectType.Stun);
        effect.transform.position = transform.position + offset;

        brain.SetActionState(Brain.ActionType.Attack, false);
        brain.SetActionState(Brain.ActionType.SpecialAttack, false);
        brain.SetActionState(Brain.ActionType.Dash, false);
        brain.SetActionState(Brain.ActionType.ChasePlayer, false);

        brain.isActionBusy = true;
        brain.enemy.Get<EnemyAnimator>().ResetAnimator();
        yield return new WaitForSeconds(time);

        brain.isActionBusy = false;
        EffectSystem.ReturnEffect(EffectType.Stun, effect);
    }

    private void OnDestroy()
    {
        brain.enemy.Get<EnemyHealth>().OnDamage -= Execute;
    }

    public void TriggerStun()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(DisableEnemy(stunDuration + 10f));
    }
}
