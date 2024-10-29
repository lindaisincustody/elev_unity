using System;
using System.Collections;
using UnityEngine;

public class GhostAttack : EnemyAttack
{
    [SerializeField] private EnemyProjectile projectile;
    [SerializeField] private Transform body;
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float angleOffset = 10f;

    private EnemyAnimator animator;

    private const float ANIMATION_DELAY = 0.65f;
    private const float SIMPLE_ATTACK_COOLDOWN = 0.7f;
    private const float SPECIAL_ATTACK_BURST_INVERVAL = 0.4f;
    private const float SPECIAL_ATTACK_COOLDOWN = 1.2f;

    public override void Attack(AttackRequest attackRequest)
    {
        animator = attackRequest.animator;
        FaceTarget(attackRequest.targetHealth.transform);
        switch (attackRequest.attackType)
        {
            case AttackType.Default:
                StartCoroutine(SimpleAttack(attackRequest.targetHealth, attackRequest.onAttackEnd));
                break;
            case AttackType.Special:
                StartCoroutine(ThrowProjectilesAround(projectileCount * 2, attackRequest.onAttackEnd));
                break;
        }
    }

    private IEnumerator SimpleAttack(Health targetHeatlh, Action onAttackEnd)
    {
        animator.Play(EnemyAnimator.AnimationType.Attack);

        yield return new WaitForSeconds(ANIMATION_DELAY);
        animator.ResetAnimator();

        Vector2 shootDirection = (targetHeatlh.transform.position - body.position).normalized;

        int halfProjectileCount = projectileCount / 2;
        for (int i = -halfProjectileCount; i <= halfProjectileCount; i++)
        {
            float currentAngle = i * angleOffset;
            Vector2 offsetDirection = RotateVector(shootDirection, currentAngle);

            EnemyProjectile newBolder = Instantiate(projectile, transform.position, Quaternion.identity);
            newBolder.Shoot(offsetDirection);
        }
        yield return new WaitForSeconds(SIMPLE_ATTACK_COOLDOWN);
        onAttackEnd?.Invoke();
    }

    public IEnumerator ThrowProjectilesAround(int numProjectiles, Action onAttackEnd)
    {
        float angleStep = 360f / numProjectiles;
        float currentAngle = 0f;

        for (int j = 0; j < 3; j++)
        {
            animator.PlayInstant(EnemyAnimator.AnimationType.Attack);
            yield return new WaitForSeconds(ANIMATION_DELAY);
            for (int i = 0; i < numProjectiles + j * 2; i++)
            {
                Vector2 projectileDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

                EnemyProjectile newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
                newProjectile.Shoot(projectileDirection);

                currentAngle += angleStep;
            }
            animator.ResetAnimator();
            yield return new WaitForSeconds(SPECIAL_ATTACK_BURST_INVERVAL);
        }

        yield return new WaitForSeconds(SPECIAL_ATTACK_COOLDOWN);
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
