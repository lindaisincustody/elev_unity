using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public AnimationType lastAnim { get; private set; } = AnimationType.Idle;

    private bool isDead = false;

    public void PlayAnimation(AnimationType anim)
    {
        if (isDead)
            return;

        if (anim == AnimationType.Die)
            isDead = true;

        if (!animator.gameObject.activeInHierarchy)
            return;

        animator.Play(anim.ToString());
        lastAnim = anim;
    }

    public enum AnimationType
    {
        Idle,
        Walk,
        Run,
        Attack,
        Die
    }
}
