using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Component
{
    [SerializeField] public int maxHealth = 100;

    public float currentHealth { get; set; }
    public bool isDead { get; set; }
    public System.Action OnDamage { get; set; }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();
        if (currentHealth == 0)
        {
            isDead = true;
            Die();
        }
    }

    protected virtual void UpdateHealthBar() { }

    protected virtual void Die() { }
}
