using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : Component
{
    [SerializeField] private Transform stunParticlePosition;
    private float stunDuration = 5f;

    private Coroutine coroutine;

    private Brain brain;


    private void Awake()
    {
        brain = GetComponent<Brain>();
    }

    public void Execute()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(DisableEnemy(stunDuration));
    }

    private IEnumerator DisableEnemy(float time)
    {
        var effect = EffectSystem.GetEffect(EffectType.Stun);
        effect.transform.position = stunParticlePosition.position;

        brain.ActionReset?.Invoke();
        brain.isActionBusy = true;
        brain.enemy.Get<EnemyAnimator>().ResetAnimator();
        yield return new WaitForSeconds(time);

        brain.isActionBusy = false;
        EffectSystem.ReturnEffect(EffectType.Stun, effect);
    }

    private void OnDestroy()
    {
    }
}