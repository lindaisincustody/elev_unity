using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAttack : EnemyAttack
{
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPos;
    [SerializeField] private Transform body;
    [SerializeField] private EnemyMovement movement;

    private EnemyAnimator animator;

    private const float ANIMATION_DELAY = 1f;
    private const float SIMPLE_ATTACK_COOLDOWN = 0.3f;

    public override void Attack(AttackRequest attackRequest)
    {
        FaceTarget(attackRequest.targetHealth.transform);
        animator = attackRequest.animator;
        animator.Play(EnemyAnimator.AnimationType.Attack);
        StartCoroutine(AttackSequence(attackRequest.targetHealth, attackRequest.onAttackEnd));
    }

    private IEnumerator AttackSequence(Health targetHeatlh, Action onAttackEnd)
    {
        var effect = EffectSystem.GetEffect(EffectType.Swipe);
        effect.transform.position = attackPos.position;
        effect.transform.localRotation = body.localScale.x != 1 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180f);
        yield return new WaitForSeconds(ANIMATION_DELAY);
        animator.ResetAnimator();
        targetHeatlh.TakeDamage(damageAmount);
        yield return new WaitForSeconds(SIMPLE_ATTACK_COOLDOWN);
        EffectSystem.ReturnEffect(EffectType.Swipe, effect);
        onAttackEnd?.Invoke();
    }

    private void FaceTarget(Transform target)
    {
        float directionToTarget = target.position.x - transform.position.x;

        if (directionToTarget > 0)
        {
            body.localScale = new Vector3(-1, body.localScale.y, body.localScale.z);
        }
        else if (directionToTarget < 0)
        {
            body.localScale = new Vector3(1, body.localScale.y, body.localScale.z);
        }
    }

    public override void ResetAttack()
    {
    }
}
