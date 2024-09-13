using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private Transform healthBarBackground;
    [SerializeField] private int maxHealth = 100;

    private float currentHealth;
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

    private void TakeDamage(int amount)
    {
        maxHealth -= amount;
        maxHealth = Mathf.Max(maxHealth, 0);
        UpdateHealthBar();
        if (maxHealth == 0)
            Die();
    }

    private void Die()
    {
        transform.gameObject.SetActive(false);
    }

    private void UpdateHealthBar()
    {
        // Calculate the health percentage
        float healthPercentage = currentHealth / maxHealth;

        // Update the scale of the health bar
        Vector3 newScale = initialScale;
        newScale.x /= healthPercentage;
        healthBarTransform.localScale = newScale;

        // Update the position so it decreases from right to left
        Vector3 newPosition = initialPosition;
        newPosition.x = initialPosition.x - (initialScale.x - newScale.x) / 2;
        healthBarTransform.localPosition = newPosition;
    }
}
