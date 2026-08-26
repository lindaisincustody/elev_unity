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

    public Vector2 spawnPos { get; private set; }
    public Vector2 minBound { get; set; }
    public Vector2 maxBound { get; set; }
    public float passedTime { get; set; }

    private bool isFrozen = false;
    private List<string> freezeReqeusts = new();

    private Body bodyHandler;
    private AvoidBehaviour enemyBehavior;
    private Vector2 velocity = Vector2.zero;
    private float speed;

    private float originalDaySpeed;
    private float originalNightSpeed;

    private void Start()
    {
        originalDaySpeed = daySpeed;
        originalNightSpeed = nightSpeed;

        bodyHandler = new(body);
        enemyBehavior = new (rb, minBound, maxBound);
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

        Vector2 proposedPosition = rb.position + enemyBehavior.GetDirection(directionAwayFromPlayer);

        proposedPosition.x = Mathf.Clamp(proposedPosition.x, minBound.x, maxBound.x);
        proposedPosition.y = Mathf.Clamp(proposedPosition.y, minBound.y, maxBound.y);

        target = proposedPosition;
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
            MoveTowardsTarget();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 currentPos = rb.position;
        Vector2 targetPos = target.Value;

        targetPos.x = Mathf.Clamp(targetPos.x, minBound.x, maxBound.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minBound.y, maxBound.y);

        rb.position = Vector2.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime, speed, Time.fixedDeltaTime);

        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            animator.Play(EnemyAnimator.AnimationType.Idle);
            target = null;
        }

        bodyHandler.UpdateBody(_target);
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
        Vector2 targetPosition = context.target.position;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBound.x, maxBound.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBound.y, maxBound.y);

        Vector2 direction = (targetPosition - rb.position).normalized;

        if (!isFrozen)
            rb.linearVelocity = direction * dashSpeed;

        StartCoroutine(StopDashAfterTime(0.2f, OnEnd)); 
    }

    public void Stop()
    {
        StopAllCoroutines();
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

}
