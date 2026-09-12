using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Component
{
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
            animator.Play(EnemyAnimator.AnimationType.Walk);
            bodyHandler.UpdateBody(_target);
        }
    }

    [SerializeField] private Transform body;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyAnimator animator;
    [SerializeField] private float daySpeed;
    [SerializeField] private float nightSpeed;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private LayerMask pathObstacles = 1;

    public Vector2 spawnPos { get; private set; }
    public Vector2 minBound => enemy.minBound;
    public Vector2 maxBound => enemy.maxBound;
    public float passedTime { get; set; }

    private const float DASH_DURATION = 0.2f;

    private bool isFrozen = false;
    private List<string> freezeReqeusts = new();

    private readonly List<Vector2> path = new List<Vector2>();
    private int pathIndex;
    private Vector2 pathGoal;
    private float pathTime;

    private static readonly Collider2D[] OverlapBuffer = new Collider2D[1];

    private Collider2D bodyCollider;
    private ContactFilter2D obstacleFilter;
    private bool pathBlocked;
    private bool movePending;
    private Body bodyHandler;
    private AvoidBehaviour enemyBehavior;
    private Vector2 velocity = Vector2.zero;
    private float speed;

    private Enemy enemy => (Enemy)Entity;

    private float originalDaySpeed;
    private float originalNightSpeed;

    private void Start()
    {
        originalDaySpeed = daySpeed;
        originalNightSpeed = nightSpeed;

        foreach (Collider2D collider2D in rb.GetComponents<Collider2D>())
        {
            if (!collider2D.isTrigger)
                bodyCollider = collider2D;
        }

        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        obstacleFilter = new ContactFilter2D { useTriggers = false, useLayerMask = true, layerMask = pathObstacles };

        bodyHandler = new(body);
        enemyBehavior = new (rb);
        spawnPos = transform.position;
        target = Vector2.zero;

        SanityManager.Instance.OnSanityChanged += SanityChange;
        SanityChange(0);

    }

    public void Freeze(string requesterId)
    {
        if (freezeReqeusts.Contains(requesterId))
            return;

        isFrozen = true;
        freezeReqeusts.Add(requesterId);
    }

    public void Unfreeze(string requesterId)
    {
        if (!freezeReqeusts.Contains(requesterId))
            return;

        freezeReqeusts.Remove(requesterId);

        if (freezeReqeusts.Count == 0)
            isFrozen = false;
    }

    private void SanityChange(int amount)
    {
        speed = SanityManager.Instance.IsPlayerInUnderworld ? nightSpeed : daySpeed;
    }

    public void Avoid(Transform player)
    {
        Vector2 directionAwayFromPlayer = (rb.position - (Vector2)player.position).normalized;

        Vector2 proposedPosition = rb.position + enemyBehavior.GetDirection(directionAwayFromPlayer, minBound, maxBound);

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
                animator.Play(EnemyAnimator.AnimationType.Idle);
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
        if (minBound == maxBound)
            return;

        if (Time.time - pathTime < 0.1f)
            return;

        if (Time.time - pathTime < 0.5f && Vector2.Distance(goal, pathGoal) < 0.5f)
            return;

        NavGrid grid = NavGridCache.Get(minBound, maxBound, ColliderOffset(), ColliderRadius(), pathObstacles);

        pathGoal = goal;
        pathTime = Time.time;
        pathIndex = 0;

        pathBlocked = !grid.FindPath(rb.position, goal, path);

        if (pathBlocked)
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

    public void Dash(Context context, float dashSpeed, System.Action OnEnd)
    {
        FaceTarget(context.target);
        Vector2 targetPosition = ClampToBounds(context.target.position);

        Vector2 toTarget = targetPosition - rb.position;
        float dashDistance = Mathf.Min(dashSpeed * DASH_DURATION, toTarget.magnitude);

        if (!isFrozen)
            rb.linearVelocity = toTarget.normalized * dashSpeed;

        StartCoroutine(StopDashAfterTime(dashDistance / dashSpeed, OnEnd));
    }

    public void Stop()
    {
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator StopDashAfterTime(float time, System.Action OnEnd)
    {
        yield return new WaitForSeconds(time);
        rb.linearVelocity = Vector2.zero;
        OnEnd?.Invoke();
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
        if (!Application.isPlaying || minBound == maxBound)
            return;

        NavGrid grid = NavGridCache.Get(minBound, maxBound, ColliderOffset(), ColliderRadius(), pathObstacles);

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
        Gizmos.DrawWireCube((minBound + maxBound) * 0.5f, maxBound - minBound);
    }

    private Vector2 ClampToBounds(Vector2 position)
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
}
