using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
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
            animator.PlayAnimation(EnemyAnimator.AnimationType.Walk);
            UpdateBody();
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

    private Vector2 velocity = Vector2.zero;
    private float speed;

    private void Start()
    {
        spawnPos = transform.position;
        target = Vector2.zero;
        
        SanityBar.instance.OnSanityChange += SanityChange;
        SanityChange(0);
    }

    private void SanityChange(int amount)
    {
        speed = SanityEffectHandler.IsPlayerInUnderworld ? nightSpeed : daySpeed;
    }

    public void Move()
    {
        if (target.HasValue)
        {
            MoveTowardsTarget();
        }
        else
        {
            rb.velocity = Vector2.zero;
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
            animator.PlayAnimation(EnemyAnimator.AnimationType.Idle);
            target = null;
        }

        UpdateBody();
    }

    private void UpdateBody()
    {
        if (!target.HasValue)
            return;

        Vector2 targetPos = target.Value;
        Vector2 currentPos = transform.position;

        if (targetPos.x < currentPos.x)
        {
            body.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            body.localScale = new Vector3(-1, 1, 1);
        }
    }
}
