using System;
using System.Collections;
using UnityEngine;

public class GhostAttack : EnemyAttack
{
    [SerializeField] private EnemyProjectile projectile;
    [SerializeField] private Transform body;
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float angleOffset = 10f;

    public override void Attack(Health targetHeatlh, EnemyAnimator animator, Action onAttackEnd)
    {
        FaceTarget(targetHeatlh.transform);
        animator.PlayAnimation(EnemyAnimator.AnimationType.Attack);
        StartCoroutine(AttackSequence(targetHeatlh, onAttackEnd));
    }

    private IEnumerator AttackSequence(Health targetHeatlh, Action onAttackEnd)
    {
        yield return new WaitForSeconds(0.4f);

        Vector2 shootDirection = (targetHeatlh.transform.position - body.position).normalized;

        int halfProjectileCount = projectileCount / 2;
        for (int i = -halfProjectileCount; i <= halfProjectileCount; i++)
        {
            float currentAngle = i * angleOffset;
            Vector2 offsetDirection = RotateVector(shootDirection, currentAngle);

            EnemyProjectile newBolder = Instantiate(projectile, transform.position, Quaternion.identity);
            newBolder.Shoot(offsetDirection);
        }

        yield return new WaitForSeconds(1.2f);
        onAttackEnd?.Invoke();
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
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
