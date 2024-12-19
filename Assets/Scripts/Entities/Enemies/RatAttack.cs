using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAttack : EnemyAttack
{
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPos;
    [SerializeField] private Transform body;

    private EnemyMovement movement;
    private EnemyAnimator animator;

    private const float SWIPE_APPEAR_DELAY = 1.1f;
    private const float SWIPE_DURATION = 0.2f;
    private const float ANIMATION_DURATION = 1.5f;

    private void Start()
    {
        movement = Enemy.Get<EnemyMovement>();
    }

    public override void Attack(Context context, AttackRequest attackRequest)
    {
        movement.FaceTarget(attackRequest.targetHealth.transform);
        animator = attackRequest.animator;
        animator.Play(EnemyAnimator.AnimationType.Attack);
        switch (attackRequest.attackType)
        {
            case AttackType.Special:
                StartCoroutine(SpecialAttack(attackRequest.targetHealth, attackRequest.onAttackEnd));
                break;
            default:
                StartCoroutine(DefaultAttack(attackRequest.targetHealth, attackRequest.onAttackEnd));
                break;
        }
    }

    private IEnumerator SpecialAttack(Health targetHealth, Action onAttackEnd)
    {
        float distance = Vector3.Distance(attackPos.position, body.position);

        var effect = EffectSystem.GetEffect(EffectType.WhiteSlashFull);
        effect.SetActive(false);
        Swipe swipe = effect.GetComponent<Swipe>();
        swipe.Init(targetHealth, damageAmount);

        effect.transform.position = body.position;

        StartCoroutine(ActivateSwipe(effect, swipe));

        yield return new WaitForSeconds(ANIMATION_DURATION);
        onAttackEnd?.Invoke();
    }

    private IEnumerator DefaultAttack(Health targetHealth, Action onAttackEnd)
    {
        float distance = Vector3.Distance(attackPos.position, body.position);

        SpawnSwipe(targetHealth, Vector3.left, distance);
        SpawnSwipe(targetHealth, Vector3.right, distance);

        yield return new WaitForSeconds(ANIMATION_DURATION);
        onAttackEnd?.Invoke();
    }

    private void SpawnSwipe(Health targetHealth, Vector3 direction, float distance)
    {
        var effect = EffectSystem.GetEffect(EffectType.WhiteSlash);
        effect.SetActive(false);
        Swipe swipe = effect.GetComponent<Swipe>();
        swipe.Init(targetHealth, damageAmount);

        Vector3 swipePosition = body.position + direction * distance;
        effect.transform.position = swipePosition;

        if (direction == Vector3.left)
        {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }

        StartCoroutine(ActivateSwipe(effect, swipe));
    }


    private IEnumerator ActivateSwipe(GameObject effect, Swipe swipe)
    {
        yield return new WaitForSeconds(SWIPE_APPEAR_DELAY);
        animator.ResetAnimator();
        effect.SetActive(true);
        yield return new WaitForSeconds(SWIPE_APPEAR_DELAY - SWIPE_DURATION);
        swipe.ActivateCollider();
        EffectSystem.ReturnEffect(EffectType.Swipe, effect);
    }

    public override void ResetAttack()
    {
    }

    public override void Stop()
    {
        StopAllCoroutines();
        animator.ResetAnimator();
    }
}
