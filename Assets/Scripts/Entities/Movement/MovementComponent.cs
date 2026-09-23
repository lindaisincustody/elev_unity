using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementComponent : Component
{
    [SerializeField] protected Transform body;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected float smoothTime = 0.3f;
    [SerializeField] protected LayerMask pathObstacles = 1;

    public virtual Vector2 minBound => Vector2.zero;
    public virtual Vector2 maxBound => Vector2.zero;

    public virtual Vector2 navMin => minBound;
    public virtual Vector2 navMax => maxBound;

    public Vector2 spawnPos { get; private set; }
    public float passedTime { get; set; }

    private Vector2? _target;

    public Vector2? target
    {
        get
        {
            return _target;
        }
        set
        {
            if (value == _target)
                return;

            _target = value;
            PlayWalk();
            bodyHandler.UpdateBody(_target);
        }
    }

    protected bool isFrozen = false;
    protected float speed;
    protected Collider2D bodyCollider;
    protected Body bodyHandler;
    protected AvoidBehaviour avoidBehaviour;

    private readonly List<Vector2> path = new List<Vector2>();
    private readonly List<string> freezeRequests = new();

    private ContactFilter2D obstacleFilter;
    private int pathIndex;
    private Vector2 pathGoal;
    private float pathTime;
    private bool pathBlocked;
    private bool movePending;
    private Vector2 velocity = Vector2.zero;

    protected abstract void PlayWalk();
    protected abstract void PlayIdle();

    protected virtual void Awake()
    {
        foreach (Collider2D collider2D in rb.GetComponents<Collider2D>())
        {
            if (!collider2D.isTrigger)
                bodyCollider = collider2D;
        }

        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        obstacleFilter = new ContactFilter2D { useTriggers = false, useLayerMask = true, layerMask = pathObstacles };

        bodyHandler = new(body);
        avoidBehaviour = new(rb);
        spawnPos = transform.position;
    }

    public void Freeze(string requesterId)
    {
        if (freezeRequests.Contains(requesterId))
            return;

        isFrozen = true;
        freezeRequests.Add(requesterId);
    }

    public void Unfreeze(string requesterId)
    {
        if (!freezeRequests.Contains(requesterId))
            return;

        freezeRequests.Remove(requesterId);

        if (freezeRequests.Count == 0)
            isFrozen = false;
    }

    public void Avoid(Transform threat)
    {
        Vector2 directionAwayFromThreat = (rb.position - (Vector2)threat.position).normalized;

        Vector2 proposedPosition = rb.position + avoidBehaviour.GetDirection(directionAwayFromThreat, minBound, maxBound);

        target = ClampToBounds(proposedPosition);
        Move();
    }

    public void Move()
    {
        if (isFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (target.HasValue)
        {
            movePending = true;
        }
        else
        {
            movePending = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!movePending || !target.HasValue || isFrozen)
            return;

        movePending = false;
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector2 goal = ClampToBounds(target.Value);

        RefreshPath(goal);

        bool onFinalWaypoint = pathIndex >= path.Count - 1;
        Vector2 waypoint = path.Count > 0 ? path[pathIndex] : goal;

        Vector2 nextPosition = Vector2.SmoothDamp(rb.position, waypoint, ref velocity, smoothTime, speed, Time.fixedDeltaTime);

        rb.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, waypoint) < (onFinalWaypoint ? 0.1f : 0.35f))
        {
            if (onFinalWaypoint)
            {
                PlayIdle();
                target = null;
                path.Clear();
            }
            else
            {
                pathIndex++;
            }
        }

        bodyHandler.UpdateBody(waypoint);
    }

    private void RefreshPath(Vector2 goal)
    {
        if (navMin == navMax)
            return;

        if (Time.time - pathTime < 0.1f)
            return;

        if (Time.time - pathTime < 0.5f && Vector2.Distance(goal, pathGoal) < 0.5f)
            return;

        NavGrid grid = NavGridCache.Get(navMin, navMax, ColliderOffset(), ColliderRadius(), pathObstacles);

        pathGoal = goal;
        pathTime = Time.time;
        pathIndex = 0;

        pathBlocked = !grid.FindPath(rb.position, goal, path);

        if (pathBlocked)
            path.Clear();
    }

    private Vector2 ColliderOffset()
    {
        return (Vector2)bodyCollider.bounds.center - rb.position;
    }

    private float ColliderRadius()
    {
        Vector3 extents = bodyCollider.bounds.extents;

        return Mathf.Max(extents.x, extents.y);
    }

    public void FaceTarget(Transform target)
    {
        float directionToTarget = target.position.x - transform.position.x;

        if (directionToTarget > 0)
        {
            body.localScale = new Vector3(-1, body.localScale.y, body.localScale.z);
        }
        else if (directionToTarget < 0)
        {
            body.localScale = new Vector3(1, body.localScale.y, body.localScale.z);
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
    }

    public void ApplySlow(float factor, float duration)
    {
        factor = Mathf.Clamp01(factor);

        float prevSpeed = speed;

        speed = prevSpeed * factor;

        StartCoroutine(SlowRoutine(prevSpeed, duration));
    }

    private IEnumerator SlowRoutine(float originalSpeed, float duration)
    {
        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
    }

    public Vector2 ClampToNavArea(Vector2 position)
    {
        if (navMin == navMax)
            return position;

        float radius = ColliderRadius();

        position.x = Mathf.Clamp(position.x, navMin.x + radius, navMax.x - radius);
        position.y = Mathf.Clamp(position.y, navMin.y + radius, navMax.y - radius);

        return position;
    }

    protected Vector2 ClampToBounds(Vector2 position)
    {
        if (minBound == maxBound)
            return position;

        Bounds colliderBounds = bodyCollider.bounds;
        Vector2 offset = (Vector2)colliderBounds.center - rb.position;
        Vector2 extents = colliderBounds.extents;

        position.x = Mathf.Clamp(position.x, minBound.x + extents.x - offset.x, maxBound.x - extents.x - offset.x);
        position.y = Mathf.Clamp(position.y, minBound.y + extents.y - offset.y, maxBound.y - extents.y - offset.y);

        return position;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || path.Count == 0)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(rb.position, path[pathIndex]);

        for (int i = pathIndex; i < path.Count - 1; i++)
            Gizmos.DrawLine(path[i], path[i + 1]);

        foreach (Vector2 point in path)
            Gizmos.DrawWireSphere(point, 0.08f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(path[pathIndex], 0.16f);

        if (target.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(target.Value, Vector3.one * 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || navMin == navMax)
            return;

        NavGrid grid = NavGridCache.Get(navMin, navMax, ColliderOffset(), ColliderRadius(), pathObstacles);

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid.IsBlocked(x, y))
                    Gizmos.DrawCube(grid.CellCenter(x, y), Vector3.one * NavGrid.CellSize * 0.9f);
            }
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube((navMin + navMax) * 0.5f, navMax - navMin);
    }
}
