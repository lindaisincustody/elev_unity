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

    public float DeathDuration => 2f;
    public bool IsAlive => currentHealth > 0;
    public bool Immune { get; set; }
    public Action OnDeath;
    public Action OnLethal;

    private Vector3 initialScale;
    private Vector3 initialPosition;

    private void Awake()
    {
        currentHealth = maxHealth;
        initialScale = healthBarTransform.localScale;
        initialPosition = healthBarTransform.localPosition;
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

    public override void TakeDamage(int amount)
    {
        if (isDead)
            return;

        if (Immune)
            return;

        OnDamage?.Invoke();
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar(FlashType.None);

        if (currentHealth == 0 && !isDead)
            StartCoroutine(Die());
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
        OnLethal?.Invoke();
        animator.Play(EnemyAnimator.AnimationType.Die);
        yield return new WaitForSeconds(DeathDuration);
        gameObject.SetActive(false);
        OnDeath?.Invoke();
    }

    protected override void UpdateHealthBar(FlashType flashType)
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
