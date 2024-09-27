using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float explosionDelay = 3f;
    [SerializeField] private int damage = 20;

    public bool Flying = false;
    public event System.Action OnBombExploded;
    public bool HasExploded { get; private set; } = false;

    private void OnEnable()
    {
        HasExploded = false;
        StartCoroutine(ExplodeAfterDelay());
    }

    public void Fly(Vector2 direction)
    {
        Flying = true;
        gameObject.SetActive(true);
        rb.velocity = direction * speed;
    }

    

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        HasExploded = true;
        OnBombExploded?.Invoke();
        Deactivate();
    }

    private void Deactivate()
    {
        Flying = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;
        transform.parent = null;
        gameObject.SetActive(false);

        // Unsubscribe all event handlers to avoid memory leaks or duplicate event calls
        OnBombExploded = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
