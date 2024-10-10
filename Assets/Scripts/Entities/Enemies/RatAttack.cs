using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAttack : EnemyAttack
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private Transform body;
    [SerializeField] private float attackRange;

    public override void Attack(Health targetHeatlh, EnemyAnimator animator, Action onAttackEnd)
    {  
        animator.PlayAnimation(EnemyAnimator.AnimationType.Attack);
        StartCoroutine(AttackSequence(targetHeatlh, onAttackEnd));
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
