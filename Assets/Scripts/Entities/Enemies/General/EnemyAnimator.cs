using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : Component
{
    [SerializeField] private Animator animator;

    public AnimationType lastAnim { get; private set; } = AnimationType.Idle;

    private bool isDead = false;

    public void Play(AnimationType anim)
    {
        if (isDead)
            return;

        if (anim == AnimationType.Die)
            isDead = true;

        if (!animator.gameObject.activeInHierarchy)
            return;

        animator.SetInteger("State", (int)anim);
        lastAnim = anim;
    }

    public void PlayInstant(AnimationType anim)
    {
        if (isDead)
            return;

        if (anim == AnimationType.Die)
            isDead = true;

        if (!animator.gameObject.activeInHierarchy)
            return;

        animator.Play(anim.ToString(), -1, 0f);  // -1 for default layer, 0f to start from the beginning
        lastAnim = anim;
    }



    public void ResetAnimator()
    {
        animator.SetInteger("State", 0);
    }

    public enum AnimationType
    {
        Idle,
        Walk,
        Attack,
        Die
    }
}
