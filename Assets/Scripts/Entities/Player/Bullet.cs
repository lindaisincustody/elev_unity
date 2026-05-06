using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private float maxDuration = 5f;
    [field: SerializeField] public int damage { get; set; }

    public bool Flying = false;
    private Vector3 startPosition;
    private Transform target; // Target for homing
    private float elapsedTime;
    private float currentDuration;

    void OnEnable()
    {
        startPosition = transform.position;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (!Flying)
            return;

        if (target != null && target.gameObject.activeInHierarchy)
        {
            HomeInOnTarget();
        }
        else
        {
            CheckDuration();
        }
    }

    public void Fly(Vector2 direction, Transform homingTarget)
    {
        Flying = true;
        gameObject.SetActive(true);

        startPosition = transform.position;
        target = homingTarget;

        rb.linearVelocity = Vector2.zero;
    }

    public void Fly(Vector2 direction, float duration)
    {
        Flying = true;
        gameObject.SetActive(true);

        startPosition = transform.position;
        target = null;

        elapsedTime = 0f;
        currentDuration = duration;

        rb.linearVelocity = direction * speed;
    }

    private void HomeInOnTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Deactivate();
        }
    }

    private void CheckDuration()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= (currentDuration > 0f ? currentDuration : maxDuration))
            Deactivate();
    }

    protected virtual void Deactivate()
    {
        Flying = false;
        rb.linearVelocity = Vector2.zero;
        target = null;
        gameObject.SetActive(false);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
