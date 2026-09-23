using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MovementComponent
{
    [SerializeField] private EnemyAnimator animator;
    [SerializeField] private float daySpeed;
    [SerializeField] private float nightSpeed;

    public override Vector2 minBound => enemy.minBound;
    public override Vector2 maxBound => enemy.maxBound;

    private const float DASH_DURATION = 0.2f;
    private const string DEATH_FREEZE = "Death";

    private Enemy enemy => (Enemy)Entity;

    protected override void PlayWalk()
    {
        animator.Play(EnemyAnimator.AnimationType.Walk);
    }

    protected override void PlayIdle()
    {
        animator.Play(EnemyAnimator.AnimationType.Idle);
    }

    private void Start()
    {
        target = Vector2.zero;

        SanityManager.Instance.OnSanityChanged += SanityChange;
        SanityChange(0);

        Entity.Get<EnemyHealth>().OnLethal += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        Stop();
        Freeze(DEATH_FREEZE);
    }

    private void OnDestroy()
    {
        SanityManager.Instance.OnSanityChanged -= SanityChange;
        Entity.Get<EnemyHealth>().OnLethal -= OnEnemyDeath;
    }

    private void SanityChange(int amount)
    {
        speed = SanityManager.Instance.IsPlayerInUnderworld ? nightSpeed : daySpeed;
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

    private IEnumerator StopDashAfterTime(float time, System.Action OnEnd)
    {
        yield return new WaitForSeconds(time);
        rb.linearVelocity = Vector2.zero;
        OnEnd?.Invoke();
    }
}
