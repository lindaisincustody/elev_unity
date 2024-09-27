using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private Transform healthBarBackground;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;

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

    public void TakeDamage(int amount)
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
