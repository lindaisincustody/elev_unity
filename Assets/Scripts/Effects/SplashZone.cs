using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SplashZone : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField][Range(0f, 1f)] private float slowFactor = 0.5f;
    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private float lifetime = 3f;

    private CircleCollider2D circleCollider;
    private bool initialSplashDone = false;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
    }

    void OnEnable()
    {
        float worldRadius = circleCollider.radius
                          * Mathf.Max(transform.lossyScale.x,
                                      transform.lossyScale.y);

        int enemyLayerMask = LayerMask.GetMask("Enemy");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            worldRadius,
            enemyLayerMask
        );

        foreach (var col in hits)
        {
            var health = col.GetComponentInParent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(damageAmount);
        }

        initialSplashDone = true;
        StartCoroutine(DestroyAfterTime());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialSplashDone) return;

        var movement = other.GetComponentInParent<EnemyMovement>();
        if (movement != null)
            movement.ApplySlow(slowFactor, slowDuration);
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (circleCollider != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
    }
}
