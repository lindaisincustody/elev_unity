using System;
using UnityEngine;

public class SlashSymbolController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public int damage = 1;

    // Now store the Enemy instance directly
    private Enemy targetEnemy;
    private bool isTackling;

    public Action<SlashSymbolController> OnTackleComplete;

    void Update()
    {
        if (!isTackling || targetEnemy == null) return;

        // Fly towards the enemy’s position
        Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Called by DiagupAbility to start the tackle.
    /// </summary>
    public void BeginTackle(Enemy enemy)
    {
        targetEnemy = enemy;
        isTackling = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTackling || targetEnemy == null) return;

        // Only deal damage when colliding with our chosen target
        if (other.gameObject == targetEnemy.gameObject)
        {
            // Call whatever damage API your Enemy exposes:
            // e.g. targetEnemy.TakeDamage(damage);
            // or if you have a health component:
            // targetEnemy.GetComponent<EnemyHealth>()?.ApplyDamage(damage);

            var health = other.GetComponent<EnemyHealth>();
            health?.TakeDamage(damage);

            // Notify the ability it’s done, then destroy
            OnTackleComplete?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
