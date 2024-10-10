using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAttack : EnemyAttack
{
    [SerializeField] private EnemyProjectile bolder;
    [SerializeField] private Transform body;

    public override void Attack(Health targetHeatlh, EnemyAnimator animator, Action onAttackEnd)
    {
        FaceTarget(targetHeatlh.transform);
        animator.PlayAnimation(EnemyAnimator.AnimationType.Attack);
        StartCoroutine(AttackSequence(onAttackEnd));
    }

    private IEnumerator AttackSequence(Action onAttackEnd)
    {
        yield return new WaitForSeconds(1f);
        EnemyProjectile newBolder = Instantiate(bolder, transform.position, Quaternion.identity);
        Vector2 shootDirection = body.localScale.x > 0 ? -body.right : body.right;
        newBolder.Shoot(shootDirection);
        yield return new WaitForSeconds(0.3f);
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
