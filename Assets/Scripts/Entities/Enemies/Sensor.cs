using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Sensor : MonoBehaviour
{
    public float detectionRadius = 10f;
    public List<string> targetTags = new();

    readonly List<Transform> detectedObjects = new(10);
    CapsuleCollider2D collider;

    private void Start()
    {
        collider = GetComponent<CapsuleCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(detectionRadius, detectionRadius);


    }

    public Transform GetClosestTarget(string tag)
    {
        if (detectedObjects.Count == 0) return null;
        Transform closestTarget = null;
        float cloesstTargetDist = Mathf.Infinity;
        Vector2 currecntPosition = transform.position;
        foreach (Transform potentialTarget in detectedObjects)
        {
            if (potentialTarget.CompareTag(tag))
            {
                Vector2 directionToTarget = (Vector2)potentialTarget.position - currecntPosition;
                float sqrToTarget = directionToTarget.sqrMagnitude;
                if (sqrToTarget < cloesstTargetDist)
                {
                    cloesstTargetDist = sqrToTarget;
                    closestTarget = potentialTarget;
                }

            }
        }
        return closestTarget;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProccessTrigger(collision, transform => detectedObjects.Add(transform));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ProccessTrigger(collision, transform => detectedObjects.Remove(transform));
    }

    void ProccessTrigger(Collider2D other, Action<Transform> action)
    {
        if (other.CompareTag("Untagged") || other.transform == transform) return;

        foreach (string t in targetTags)
        {
            if (other.CompareTag(t))
            {
                action(other.transform);
            }
        }
    }
}
