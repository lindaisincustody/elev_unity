using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 100;

    protected float currentHealth;
    protected bool isDead = false;

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
