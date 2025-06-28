using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] public float explosionDelay = 3f;
    [SerializeField] public int damage = 20;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 1.5f);

    public bool Flying = false;

    public void Init()
    {
        StartCoroutine(FuseAndScale());
        StartCoroutine(ExplodeAfterDelay());
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

        GameObject effect = EffectSystem.GetEffect(EffectType.Explosion);
        effect.transform.position = transform.position;
        effect.SetActive(true);
        CoroutineRunner.Instance.StartCoroutine(EffectSystem.ReturnEffectOnComplete(EffectType.Explosion, effect));
        Destroy(gameObject);
    }

    private IEnumerator FuseAndScale()
    {
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < explosionDelay)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / explosionDelay);

            float scaleMultiplier = scaleCurve.Evaluate(t);
            transform.localScale = originalScale * scaleMultiplier;

            yield return null;
        }

        Explode();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
