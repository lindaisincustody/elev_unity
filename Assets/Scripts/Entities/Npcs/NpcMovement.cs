using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NpcMovement : MovementComponent
{
    [SerializeField] private NpcAnimator animator;
    [SerializeField] private float walkSpeed = 2f;

    private Vector2 areaMin;
    private Vector2 areaMax;

    public override Vector2 navMin => areaMin;
    public override Vector2 navMax => areaMax;

    protected override void Awake()
    {
        avoidanceLayers |= LayerMask.GetMask("Player");

        base.Awake();

        speed = walkSpeed;
    }

    public void SetArea(Vector2 min, Vector2 max)
    {
        areaMin = min;
        areaMax = max;
    }

    public async UniTask MoveTo(Vector2 destination, CancellationToken token)
    {
        target = destination;

        await UniTask.WaitUntil(() => !target.HasValue, cancellationToken: token);

        target = null;
    }

    public async UniTask WalkInto(Vector2 destination, CancellationToken token)
    {
        target = null;
        Freeze(nameof(WalkInto));
        bodyCollider.enabled = false;

        PlayWalk();
        FaceDirection(destination - rb.position);

        await Glide(destination, speed, token);

        PlayIdle();
    }

    public async UniTask Shake(Vector2 amplitude, float frequency, float duration, CancellationToken token)
    {
        Vector2 origin = rb.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            await UniTask.WaitForFixedUpdate(token);

            elapsed += Time.fixedDeltaTime;

            float phase = elapsed * frequency * 2f * Mathf.PI;

            rb.MovePosition(origin + Vector2.Scale(amplitude, new Vector2(Mathf.Sin(phase), Mathf.Sin(phase * 2f))));
        }

        rb.MovePosition(origin);
    }

    public async UniTask Glide(Vector2 destination, float glideSpeed, CancellationToken token)
    {
        while (rb.position != destination)
        {
            await UniTask.WaitForFixedUpdate(token);

            rb.MovePosition(Vector2.MoveTowards(rb.position, destination, glideSpeed * Time.fixedDeltaTime));
        }
    }

    protected override void PlayWalk()
    {
        animator.Play(NpcAnimator.AnimationType.Walk);
    }

    protected override void PlayIdle()
    {
        animator.Play(NpcAnimator.AnimationType.Idle);
    }

    protected override void FaceDirection(Vector2 direction)
    {
        animator.SetDirection(direction);
    }
}
