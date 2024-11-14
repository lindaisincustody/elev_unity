using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 2f;
    [field: SerializeField] public int damage { get; private set; }

    public bool Flying = false;
    private Vector3 startPosition;
    private Transform target; // Target for homing

    void OnEnable()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (Flying)
        {
            if (target != null)
            {
                HomeInOnTarget();
            }
            else
            {
                CheckDistance();
            }
        }
    }

    public void Fly(Vector2 direction, Transform homingTarget = null)
    {
        Flying = true;
        gameObject.SetActive(true);
        startPosition = transform.position;

        // Set target if provided
        target = homingTarget;

        if (target == null)
        {
            rb.velocity = direction * speed; // Straight flight if no target
        }
        else
        {
            rb.velocity = Vector2.zero; // Reset velocity for homing
        }
    }

    private void HomeInOnTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            target.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Deactivate();
        }
    }

    private void CheckDistance()
    {
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        Flying = false;
        rb.velocity = Vector2.zero;
        target = null;
        gameObject.SetActive(false);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
