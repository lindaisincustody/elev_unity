using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAttack : EnemyAttack
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform body;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float shakeIntensity = 0.1f;

    private Coroutine shakeCoruotine;
    private EnemyAnimator animator;

    private float attackSpeed = 16f;
    private Vector3 originalPosition;
    private Vector3 beforeShakePosition;

    private bool canAttack = false;

    public override void Attack(Context context, AttackRequest attackRequest)
    {
        originalPosition = transform.position;
        animator = attackRequest.animator;
        if (canAttack)
        {
            animator.Play(EnemyAnimator.AnimationType.Attack);
            StartCoroutine(QuickAttack(attackRequest.targetHealth, attackRequest.onAttackEnd));
        }
        else
            PrepareAttack(attackRequest.targetHealth.transform, attackRequest.onAttackEnd);
    }

    private void PrepareAttack(Transform target, System.Action onAttackEnd)
    {
        beforeShakePosition = transform.position;
        FaceTarget(target);
        shakeCoruotine = StartCoroutine(ChangeColorAndShake(onAttackEnd));
    }

    private IEnumerator ChangeColorAndShake(System.Action OnAttackEnd)
    {
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < duration)
        {
            spriteRenderer.color = Color.Lerp(originalColor, Color.red, elapsed / duration);

            transform.localPosition = beforeShakePosition + (Vector3)(Random.insideUnitCircle * shakeIntensity);

            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = Color.white;
        transform.localPosition = beforeShakePosition;
        canAttack = true;

        OnAttackEnd?.Invoke();
    }

    private IEnumerator QuickAttack(Health targetHealth, System.Action OnAttackEnd)
    {
        Vector3 targetPosition = targetHealth.transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float attackDuration = distanceToTarget / attackSpeed;

        for (float t = 0; t < attackDuration; t += Time.deltaTime)
        {
            float progress = Mathf.Sin((t / attackDuration) * Mathf.PI / 2);
            transform.position = Vector3.Lerp(originalPosition, targetPosition, progress);
            yield return null;
        }

        targetHealth.TakeDamage(damageAmount);

        for (float t = 0; t < attackDuration; t += Time.deltaTime)
        {
            float progress = Mathf.Sin((t / attackDuration) * Mathf.PI / 2);
            transform.position = Vector3.Lerp(targetPosition, originalPosition, progress);
            yield return null;
        }

        OnAttackEnd?.Invoke();
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

    public override void Stop()
    {
        ResetAttack();
    }

    public override void ResetAttack()
    {
        if (shakeCoruotine != null)
            StopCoroutine(shakeCoruotine);

        spriteRenderer.color = Color.white;
        canAttack = false;
    }
}
