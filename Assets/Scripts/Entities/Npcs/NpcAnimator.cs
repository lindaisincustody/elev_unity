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

    public void SetDirection(Vector2 direction)
    {
        if (animator == null || direction.sqrMagnitude < 0.000001f)
            return;

        Vector2 facing = Facing.ToAxis(direction);

        animator.SetFloat("Horizontal", facing.x);
        animator.SetFloat("Vertical", facing.y);
    }

    public enum AnimationType
    {
        Idle,
        Walk,
        Interact
    }
}
