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
