using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public AnimationType lastAnim { get; private set; } = AnimationType.Idle;

    public void PlayAnimation(AnimationType anim)
    {
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
