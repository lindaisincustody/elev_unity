using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAttack : EnemyAttack
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private Transform body;
    [SerializeField] private float attackRange;

    public override void Attack(AttackRequest attackRequest)
    {
        attackRequest.animator.Play(EnemyAnimator.AnimationType.Attack);
        StartCoroutine(AttackSequence(attackRequest.targetHealth, attackRequest.onAttackEnd));
    }

    private IEnumerator AttackSequence(Health targetHeatlh, Action onAttackEnd)
    {
        yield return new WaitForSeconds(1f);
        targetHeatlh.TakeDamage(damageAmount);
        yield return new WaitForSeconds(0.3f);
        onAttackEnd?.Invoke();
    }

    public override void ResetAttack()
    {
    }
}
