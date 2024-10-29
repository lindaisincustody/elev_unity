using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : Component
{
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
        effect.transform.position = transform.position;
        brain.isActionBusy = true;
        yield return new WaitForSeconds(time);
        brain.isActionBusy = false;
        EffectSystem.ReturnEffect(EffectType.Stun, effect);
    }

    private void OnDestroy()
    {
        brain.enemy.Get<EnemyHealth>().OnDamage -= Execute;
    }
}
