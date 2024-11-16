using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatSpecialAttack : Component
{
    private float _cooldown = 5f;
    private RatAttack ratAttack;
    private EnemyMovement enemyMovement;

    private void Start()
    {
        ratAttack = Enemy.Get<RatAttack>();
        enemyMovement = Enemy.Get<EnemyMovement>();
    }

    public void Execute(Context context, System.Action OnEnd)
    {
        StartCoroutine(AttackSequence(context, OnEnd));
    }

    private IEnumerator AttackSequence(Context context, System.Action OnEnd)
    {
        context.brain.SetActionState(Brain.ActionType.SpecialAttack, false);
        Dash(context, 3);
        yield return new WaitForSeconds(1f);
        Dash(context, 9);
        yield return new WaitForSeconds(1f);
        OnEnd?.Invoke();
        yield return new WaitForSeconds(_cooldown);
        context.brain.SetActionState(Brain.ActionType.SpecialAttack, true);
    }

    private void Attack(Context context, int swipeCount)
    {
        var health = context.target.GetComponent<Health>();
        var animator = context.brain.enemy.Get<EnemyAnimator>();
        var lastAnim = animator.lastAnim;
        AttackRequest attackRequest = new AttackRequest(
            health, animator, swipeCount, AttackType.Default,
                () =>
                {
                    animator.Play(lastAnim);
                }
            );

        ratAttack.Attack(context, attackRequest);
    }

    private void Dash(Context context, int swipeCount)
    {
        enemyMovement.Dash(context, 10, () => Attack(context, swipeCount));
    }
}
