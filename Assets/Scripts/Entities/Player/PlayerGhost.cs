using UnityEngine;

public class PlayerGhost : Component
{
    [SerializeField] private Collider2D bodyCollider;

    public System.Action<Collider2D> OnGhost;

    public void Toggle()
    {
        if (!bodyCollider.enabled)
        {
            bodyCollider.enabled = true;
            return;
        }
        bodyCollider.enabled = false;
        OnGhost?.Invoke(bodyCollider);
    }
}
