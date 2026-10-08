using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ElevatorNpcSpawner : MonoBehaviour
{
    [SerializeField] private NpcSpawner spawner;
    [SerializeField] private float minDelay = 10f;
    [SerializeField] private float maxDelay = 30f;

    private CancellationToken destroyToken;

    private void Start()
    {
        destroyToken = gameObject.GetCancellationTokenOnDestroy();

        ElevatorGameManager.Instance.OnPassengerLifted += HandlePassengerLifted;
    }

    private void OnDestroy()
    {
        ElevatorGameManager.Instance.OnPassengerLifted -= HandlePassengerLifted;
    }

    private void HandlePassengerLifted()
    {
        SpawnAfterDelay().Forget();
    }

    private async UniTaskVoid SpawnAfterDelay()
    {
        float delay = UnityEngine.Random.Range(minDelay, maxDelay);

        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: destroyToken);

        spawner.SpawnAll();
    }
}
