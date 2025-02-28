using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowParticleCollision : MonoBehaviour
{
    public int damage = 3;

    private void OnParticleCollision(GameObject other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Arrow particle hit {enemy.name} and dealt {damage} damage.");
            }
        }
    }
}