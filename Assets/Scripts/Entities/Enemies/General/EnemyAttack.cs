using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    public int damageAmount;
    public abstract void Attack(Health targetHeatlh, EnemyAnimator animator, System.Action onAttackEnd);
    public abstract void ResetAttack();
}
