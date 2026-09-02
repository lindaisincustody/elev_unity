using UnityEngine;

public class HollowCircle : MonoBehaviour
{
    private Animator animator;
    private HollowCircleManager manager;

    public float Angle { get; private set; }
    public RectTransform Rect { get; private set; }

    private void Awake()
    {
        Rect = (RectTransform)transform;
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void Initialize(HollowCircleManager manager, float angle)
    {
        this.manager = manager;
        Angle = angle;
    }

    public void HitHollowCircle()
    {
        animator.enabled = true;
        manager.TwitchAnimation();
        animator.SetTrigger("Hollow_trigger");

        Invoke(nameof(RemoveHollowCircleAfterDelay), 0.5f);
    }

    private void RemoveHollowCircleAfterDelay()
    {
        manager.RemoveHollowCircle(this);
    }
}
