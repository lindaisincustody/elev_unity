using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : Component
{
    public int damageAmount;
    public abstract void Attack(Context context, AttackRequest attackRequest);
    public abstract void ResetAttack();
    public abstract void Stop();
}

public struct AttackRequest
{
    public Health targetHealth;
    public EnemyAnimator animator;
    public int attackCount;
    public AttackType attackType;
    public System.Action onAttackEnd;

    public AttackRequest(Health health, EnemyAnimator anim, int attackCount, AttackType type, System.Action action)
    {
        this.targetHealth = health;
        this.animator = anim;
        this.attackCount = attackCount;
        this.attackType = type;
        this.onAttackEnd = action;
    }
}

public enum AttackType
{
    Default,
    Special
}
