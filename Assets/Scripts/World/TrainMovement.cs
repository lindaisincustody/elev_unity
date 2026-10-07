using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [SerializeField] private float initialSpeed = -30f;
    [SerializeField] private float arrivalX = -11.7f;
    [SerializeField] private float leaveX = -94f;
    [SerializeField] private float stopOvershoot = 2f;
    [SerializeField] private CameraShake cameraShake;

    private Rigidbody2D rb;
    private float velocity;

    private void Start()
    {
        PlayerSnapshot playerSnapshot = SaveLoadService.Instance.Get<GeneralSaveFile>().PlayerSnapshot;

        if (playerSnapshot.TutorialComplete)
        {
            Player.instance.ShowPlayer(true);
            gameObject.SetActive(false);
            return;
        }

        Player.instance.ShowPlayer(false);
        playerSnapshot.TutorialComplete = true;
        SaveLoadService.Instance.SaveProgress();

        rb = GetComponent<Rigidbody2D>();
        Run(gameObject.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid Run(CancellationToken token)
    {
        SoundManager.PlaySound2D(SoundManager.Sound.TrainComing);

        await Arrive(token);
        await DriftToStop(token);

        await UniTask.Delay(TimeSpan.FromSeconds(3.3f), cancellationToken: token);
        SoundManager.PlaySound2D(SoundManager.Sound.TrainLeaving);
        await UniTask.Delay(TimeSpan.FromSeconds(1.7f), cancellationToken: token);

        await Leave(token);
    }

    private async UniTask Arrive(CancellationToken token)
    {
        velocity = initialSpeed;

        while (transform.position.x > arrivalX + stopOvershoot * 2f)
        {
            Accelerate(0f, 10f);

            if (Mathf.Abs(velocity) > 5f)
                cameraShake.Shake(0.5f, 0.01f);

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }

        while (transform.position.x > arrivalX - stopOvershoot)
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
    }

    private async UniTask DriftToStop(CancellationToken token)
    {
        while (Mathf.Abs(velocity) > 0.1f)
        {
            Accelerate(0f, 5f);
            cameraShake.Shake(0.3f, 0.007f);

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }

        Stop();
    }

    private async UniTask Leave(CancellationToken token)
    {
        Player.instance.ShowPlayer(true);
        velocity = initialSpeed / 2f;

        while (transform.position.x > leaveX)
        {
            Accelerate(initialSpeed, 2.5f);

            if (Mathf.Abs(velocity) > 5f)
                cameraShake.Shake(0.2f, 0.01f);

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }

        Stop();
    }

    private void Accelerate(float target, float rate)
    {
        velocity = Mathf.MoveTowards(velocity, target, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(velocity, 0f);
    }

    private void Stop()
    {
        velocity = 0f;
        rb.linearVelocity = Vector2.zero;
    }
}
