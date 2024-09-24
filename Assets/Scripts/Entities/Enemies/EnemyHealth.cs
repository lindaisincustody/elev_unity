using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private Transform healthBarBackground;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;

    private float currentHealth;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Color originalColor;
    private Material enemyMaterial;

    private void Start()
    {
        currentHealth = maxHealth;
        initialScale = healthBarTransform.localScale;
        initialPosition = healthBarTransform.localPosition;
        originalColor = enemySpriteRenderer.color; // Store the original color
        enemyMaterial = enemySpriteRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            TakeDamage(collision.GetComponent<Bullet>().damage);
        }
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();
        StartCoroutine(FlashWhite());

        if (currentHealth == 0)
            Die();
    }

    private IEnumerator FlashWhite()
    {
        enemyMaterial.SetFloat("_FlashIntensity", 1f);
        yield return new WaitForSeconds(flashDuration);
        enemyMaterial.SetFloat("_FlashIntensity", 0f);
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    private void UpdateHealthBar()
    {
        // Calculate the health percentage
        float healthPercentage = currentHealth / maxHealth;

        // Update the scale of the health bar
        Vector3 newScale = initialScale;
        newScale.x *= healthPercentage;
        healthBarTransform.localScale = newScale;

        // Update the position so it decreases from right to left
        Vector3 newPosition = initialPosition;
        newPosition.x = initialPosition.x - (initialScale.x - newScale.x) / 2;
        healthBarTransform.localPosition = newPosition;
    }
}
