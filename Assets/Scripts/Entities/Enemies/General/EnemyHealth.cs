using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private Transform healthBarBackground;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private EnemyAnimator animator;
    [SerializeField] private float flashDuration = 1f;

    public bool Immune { get; set; }
    public Action OnDamage;
    public Action OnDeath;

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Color originalColor;
    private Material enemyMaterial;

    private void Awake()
    {
        currentHealth = maxHealth;
        initialScale = healthBarTransform.localScale;
        initialPosition = healthBarTransform.localPosition;
        originalColor = body.color; // Store the original color
        enemyMaterial = body.material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            TakeDamage(collision.GetComponent<Bullet>().damage);
        }
    }

    public void ActivateHealthBar()
    {
        healthBar.SetActive(true);
    }

    public void TakeDamage(int amount)
    {
        if (Immune)
            return;

        OnDamage?.Invoke();
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();
        StartCoroutine(FlashWhite());

        if (currentHealth == 0 && !isDead)
            StartCoroutine(Die());
    }

    public IEnumerator FlashWhite()
    {
        enemyMaterial.SetFloat("_FlashIntensity", 4f);
        yield return new WaitForSeconds(flashDuration);
        enemyMaterial.SetFloat("_FlashIntensity", 0f);
    }

    public void SetAlpha(float value)
    {
        Color bodyColor = body.color;
        bodyColor.a = value;
        body.color = bodyColor;
    }

    public IEnumerator Die()
    {
        isDead = true;
        animator.Play(EnemyAnimator.AnimationType.Die);
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        OnDeath?.Invoke();
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
