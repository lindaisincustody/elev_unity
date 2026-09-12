using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Sensor : MonoBehaviour
{
    public float detectionRadius = 10f;
    public List<string> targetTags = new();

    [SerializeField] private LayerMask sightBlockers = 1;
    [SerializeField] private float memoryDuration = 2f;

    [SerializeField] List<Transform> detectedObjects = new(10);
    CapsuleCollider2D collider;

    private static readonly RaycastHit2D[] SightHits = new RaycastHit2D[1];

    private readonly Dictionary<string, Memory> memories = new();
    private ContactFilter2D sightFilter;
    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();

        collider = GetComponent<CapsuleCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(detectionRadius, detectionRadius);

        sightFilter = new ContactFilter2D { useTriggers = false, useLayerMask = true, layerMask = sightBlockers };

        Player.instance.OnGhost += OnTriggerExit2D;
    }

    public bool CanSee(Transform target)
    {
        if (!IsInsideArena(target.position))
            return false;

        Vector2 origin = transform.position;
        Vector2 toTarget = (Vector2)target.position - origin;

        return Physics2D.Raycast(origin, toTarget.normalized, sightFilter, SightHits, toTarget.magnitude) == 0;
    }

    private bool IsInsideArena(Vector2 position)
    {
        return enemy.IsInsideBounds(position);
    }

    public Transform GetClosestTarget(string tag)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;
        Vector2 currentPosition = transform.position;

        foreach (Transform potentialTarget in detectedObjects)
        {
            if (potentialTarget == null || !potentialTarget.CompareTag(tag) || !CanSee(potentialTarget))
                continue;

            float sqrToTarget = ((Vector2)potentialTarget.position - currentPosition).sqrMagnitude;

            if (sqrToTarget < closestDistance)
            {
                closestDistance = sqrToTarget;
                closestTarget = potentialTarget;
            }
        }

        if (closestTarget != null)
        {
            memories[tag] = new Memory(closestTarget, closestTarget.position, Time.time);
            return closestTarget;
        }

        if (memories.TryGetValue(tag, out Memory memory) && Time.time - memory.SeenAt < memoryDuration)
            return memory.Target;

        return null;
    }

    public bool TryGetLastSeenPosition(string tag, out Vector2 position)
    {
        if (memories.TryGetValue(tag, out Memory memory) && Time.time - memory.SeenAt < memoryDuration)
        {
            position = memory.Position;
            return true;
        }

        position = Vector2.zero;

        return false;
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

    private void OnDestroy()
    {
        Player.instance.OnGhost -= OnTriggerExit2D;
    }

    private readonly struct Memory
    {
        public readonly Transform Target;
        public readonly Vector2 Position;
        public readonly float SeenAt;

        public Memory(Transform target, Vector2 position, float seenAt)
        {
            Target = target;
            Position = position;
            SeenAt = seenAt;
        }
    }
}
