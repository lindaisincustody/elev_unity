using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Component
{
    [SerializeField] public int maxHealth = 100;

    public float currentHealth { get; set; }
    public bool isDead { get; set; }
    public System.Action OnDamage { get; set; }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar(FlashType.Damage);
        if (currentHealth == 0)
        {
            isDead = true;
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthBar(FlashType.Heal);
    }

    protected virtual void UpdateHealthBar(FlashType flashType) { }

    protected virtual void Die() { }
}

public enum FlashType
{
    Damage,
    Heal,
    None
}
