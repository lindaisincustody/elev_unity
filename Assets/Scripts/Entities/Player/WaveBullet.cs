using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaveBullet : Bullet
{
    public float Force { get; set; }
    private readonly List<Rigidbody2D> _dragged = new List<Rigidbody2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Flying) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(damage);

            Rigidbody2D enemyRb = other.attachedRigidbody;
            if (enemyRb != null && !_dragged.Contains(enemyRb))
                _dragged.Add(enemyRb);
        }
    }

    private void FixedUpdate()
    {
        if (!Flying || _dragged.Count == 0) return;

        Vector2 v = rb.linearVelocity;
        foreach (Rigidbody2D enemyRb in _dragged)
        {
            if (enemyRb != null)
                enemyRb.linearVelocity = v * Force;
        }
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        _dragged.Clear();
    }
}
