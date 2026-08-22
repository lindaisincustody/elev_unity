using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMover : MonoBehaviour
{
    private Vector3 target;
    private float speed;

    public void Initialize(Vector3 targetPosition, float speed)
    {
        target = targetPosition;
        this.speed = speed;
    }

    void Update()
    {
        if (target == null) return;

        // Move the particle toward the target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Destroy the particle if it reaches the target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}


