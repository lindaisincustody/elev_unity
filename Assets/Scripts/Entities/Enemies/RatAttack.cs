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

    private float angleIncrement = 30f;

    private const float ANIMATION_DELAY = 1f;
    private const float SIMPLE_ATTACK_COOLDOWN = 0.3f;

    private void Start()
    {
        movement = Enemy.Get<EnemyMovement>();
    }

    public override void Attack(Context context, AttackRequest attackRequest)
    {
        movement.FaceTarget(attackRequest.targetHealth.transform);
        animator = attackRequest.animator;
        animator.Play(EnemyAnimator.AnimationType.Attack);
        StartCoroutine(AttackSequence(attackRequest.targetHealth, attackRequest.onAttackEnd, attackRequest.attackCount));
    }

    private IEnumerator AttackSequence(Health targetHealth, Action onAttackEnd, int swipeCount)
    {
        float distance = Vector3.Distance(attackPos.position, body.position);

        // Determine the base direction based on the enemy's facing direction
        Vector3 baseDirection = body.localScale.x < 0 ? Vector3.right : Vector3.left;

        for (int i = 0; i < swipeCount; i++)
        {
            var effect = EffectSystem.GetEffect(EffectType.Swipe);
            Swipe swipe = effect.GetComponent<Swipe>();
            swipe.Init(targetHealth, damageAmount);

            // Calculate the angle for this swipe
            float angle = i % 2 == 0 ? (i / 2) * angleIncrement : -(i / 2 + 1) * angleIncrement;

            // Rotate the base direction by the calculated angle
            Vector3 offset = Quaternion.Euler(0, 0, angle) * baseDirection * distance;
            Vector3 swipePosition = body.position + offset;

            // Set the swipe's position and rotation
            effect.transform.position = swipePosition;

            // Adjust rotation of swipe based on angle and flip
            effect.transform.rotation = Quaternion.Euler(0, 0, angle + (body.localScale.x < 0 ? 0 : 180f));

            // Activate the collider after the animation delay
            StartCoroutine(ActivateSwipe(effect, swipe));

            yield return new WaitForSeconds(SIMPLE_ATTACK_COOLDOWN / swipeCount); // Slight delay between spawns
        }

        animator.ResetAnimator();
        yield return new WaitForSeconds(SIMPLE_ATTACK_COOLDOWN);
        onAttackEnd?.Invoke();
    }


    private IEnumerator ActivateSwipe(GameObject effect, Swipe swipe)
    {
        yield return new WaitForSeconds(ANIMATION_DELAY);
        swipe.ActivateCollider();
        EffectSystem.ReturnEffect(EffectType.Swipe, effect);
    }

    public override void ResetAttack()
    {
    }
}
