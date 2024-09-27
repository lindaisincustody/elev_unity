using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private Transform healthBarBackground;

    private Vector3 initialScale;
    private Vector3 initialPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            TakeDamage(collision.GetComponent<Bullet>().damage);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        initialScale = healthBarTransform.localScale;
        initialPosition = healthBarTransform.localPosition;
    }

    protected override void Die()
    {
        transform.gameObject.SetActive(false);
    }

    protected override void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;

        Vector3 newScale = initialScale;
        newScale.x *= healthPercentage;
        healthBarTransform.localScale = newScale;

        Vector3 newPosition = initialPosition;
        newPosition.x = initialPosition.x - (initialScale.x - newScale.x) / 2;
        healthBarTransform.localPosition = newPosition;
    }

    public float NormalizedHealth()
    {
        return currentHealth / maxHealth;
    }
}
