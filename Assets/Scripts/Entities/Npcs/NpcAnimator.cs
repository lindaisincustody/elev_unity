using UnityEngine;

public class NpcAnimator : Component
{
    [SerializeField] private Animator animator;

    public AnimationType lastAnim { get; private set; } = AnimationType.Idle;

    public void Play(AnimationType anim)
    {
        if (animator == null)
            return;

        animator.SetInteger("State", (int)anim);
        lastAnim = anim;
    }

    public enum AnimationType
    {
        Idle,
        Walk,
        Interact
    }
}
