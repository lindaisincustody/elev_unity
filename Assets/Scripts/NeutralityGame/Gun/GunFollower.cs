using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFollower : MonoBehaviour
{
    public Transform target; // The target to follow
    public float speed = 5f; // The speed at which to follow the target

    private void Start()
    {
        transform.position = target.position;
    }

    private void Update()
    {
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 direction = target.position - transform.position;
            direction.z = 0; // Ensure the movement is in 2D (zero out the z component)

            // Normalize the direction to get a unit vector
            Vector3 moveDirection = direction.normalized;

            // Calculate the distance to the target
            float distance = direction.magnitude;

            // Move towards the target at the specified speed
            float moveDistance = speed * Time.deltaTime;
            if (moveDistance > distance) // Ensure not to overshoot the target
                moveDistance = distance;

            // Move the object
            transform.Translate(moveDirection * moveDistance);
        }
    }
}
