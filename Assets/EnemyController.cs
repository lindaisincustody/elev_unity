using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform heart;
    public float movementRange = 10.0f; // The range of the movement from one side to the other
    public float moveSpeed = 5.0f; // Speed of the movement

    private bool movingRight = true; // Direction of movement
    public Vector2 targetPosition; // Target position to move towards

    void Start()
    {
        SetNewTargetPosition();
    }

    void Update()
    {
        MoveEnemy();
    }

    void SetNewTargetPosition()
    {
        // Determine the new target position based on the current direction of movement
        float targetX = heart.position.x + (movingRight ? movementRange : -movementRange);
        float targetY = heart.position.y + Random.Range(-movementRange, movementRange); // Add some randomness in Y
        targetPosition = new Vector2(targetX, targetY);

        // Toggle the movement direction
        movingRight = !movingRight;
    }

    void MoveEnemy()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If the enemy reaches the target position, set a new target
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }

        // Optionally, make the enemy face the heart or player when shooting
        Vector3 direction = heart.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}