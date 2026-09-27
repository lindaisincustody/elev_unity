using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ElevatorGameManager : CoreService
{
    private class Request
    {
        public Npc npc;
        public UniTaskCompletionSource ride = new UniTaskCompletionSource();
    }

    public static ElevatorGameManager Instance { get; private set; }

    [SerializeField] private int totalFloors = 6;
    [SerializeField] private int miniGameLevels = 3;
    [SerializeField] private int sanityCost = 50;

    private readonly List<Request> waiting = new List<Request>();

    public bool HasPassenger => waiting.Count > 0;

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public async UniTask WaitForRide(Npc npc, CancellationToken token)
    {
        Request request = new Request { npc = npc };
        waiting.Add(request);

        try
        {
            await request.ride.Task.AttachExternalCancellation(token);
        }
        finally
        {
            waiting.Remove(request);
        }
    }

    public async UniTaskVoid Play()
    {
        Request request = waiting[0];
        ElevatorCanvas canvas = UIManager.Instance.Get<ElevatorCanvas>();

        bool completed = await canvas.Ride(request.npc, Random.Range(2, totalFloors + 1), miniGameLevels);

        canvas.Close();

        if (!completed)
            return;

        SanityManager.Instance.DecreaseSanity(sanityCost);
        request.ride.TrySetResult();
    }
}
