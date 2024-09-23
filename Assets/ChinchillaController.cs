using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChinchillaController : MonoBehaviour
{
    private Transform player; // Reference to the player's transform
    private PlayerMovement playerMovement; // Reference to the player's movement script

    [SerializeField] private Bullet bulletPrefab; // Reference to the bullet prefab
    [SerializeField] private float followSpeed = 5f; // Speed at which the chinchilla follows the player
    [SerializeField] private float shootCooldown = 1f; // Time between shooting bullets
    [SerializeField] private Transform bulletSpawnPoint; // Where the bullet will be spawned
    [SerializeField] private float agroRange = 5f; // Distance within which chinchilla detects enemies
    [SerializeField] private float attackRange = 1.5f; // Distance within which chinchilla starts shooting at enemies
    [SerializeField] private LayerMask enemyLayer; // Layer for enemies
    [SerializeField] private float followSmoothTime = 0.3f; // Time for smoothing movement

    private Vector2 currentVelocity; // For smooth damping
    private float lastShootTime = 0f;
    private Vector2 lastPlayerDirection;
    private bool isSpeedBoosted = false;
    private float speedBoostEndTime = 0f;

    private Animator animator; // Reference to Animator component
    private Transform currentTarget = null; // The current enemy the chinchilla is targeting

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>(); // Get reference to the PlayerMovement script
        lastPlayerDirection = playerMovement.lastDirection; // Initialize with the player's starting direction

        // Get the Animator component attached to the chinchilla
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectEnemiesInRange();

        if (currentTarget != null)
        {
            MoveToTarget();
        }
        else
        {
            FollowPlayer(); // Follow the player if no enemies are detected
        }

        Shoot();
    }

    private void DetectEnemiesInRange()
    {
        // Detect all enemies within the agro range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, agroRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            // Find the closest enemy
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (Collider2D enemy in enemiesInRange)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy.transform;
                }
            }

            currentTarget = closestEnemy;
        }
        else
        {
            currentTarget = null;
        }
    }

    private void MoveToTarget()
    {
        if (currentTarget == null) return;

        // Move towards the target enemy
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 targetPosition = currentTarget.position;

        // Use SmoothDamp to move smoothly towards the target enemy
        Vector2 newPosition = Vector2.SmoothDamp(rb.position, targetPosition, ref currentVelocity, followSmoothTime, followSpeed);
        rb.MovePosition(newPosition);

        // Update animations based on movement direction
        UpdateAnimation(newPosition - rb.position);

        // If the chinchilla is close enough to the target, start shooting
        if (Vector2.Distance(transform.position, targetPosition) <= attackRange)
        {
            Shoot();
        }
    }

    private void FollowPlayer()
    {
        if (player == null || playerMovement == null) return;

        // Follow player logic here (same as before)
        Vector2 playerPosition = player.position;
        Vector2 currentPlayerDirection = playerMovement.lastDirection;

        // Move ahead of the player based on their direction
        Vector2 targetPosition = playerPosition + currentPlayerDirection.normalized * 1.5f; // Adjust distance as needed
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 newPosition = Vector2.SmoothDamp(rb.position, targetPosition, ref currentVelocity, followSmoothTime, followSpeed);
        rb.MovePosition(newPosition);

        UpdateAnimation(newPosition - rb.position);
    }

    private void Shoot()
    {
        if (currentTarget == null || Time.time < lastShootTime + shootCooldown) return;

        // Create the bullet and shoot it towards the target
        Bullet newBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Vector3 direction = (currentTarget.position - bulletSpawnPoint.position).normalized;
        newBullet.Fly(direction); // Assuming the Bullet class has a Fly method to set direction

        lastShootTime = Time.time; // Reset the shoot timer
    }

    private void UpdateAnimation(Vector2 direction)
    {
        // Animation logic to switch between right/left animations
        if (direction.x > 0)
        {
            animator.SetBool("isMovingRight", true);
        }
        else if (direction.x < 0)
        {
            animator.SetBool("isMovingRight", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the agro range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agroRange);

        // Visualize the attack range in the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
