using System;
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
    private readonly List<Elevator> elevators = new List<Elevator>();

    public bool HasPassenger => waiting.Count > 0;

    public event Action OnPassengerLifted;

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public void RegisterElevator(Elevator elevator)
    {
        elevators.Add(elevator);
    }

    public void UnregisterElevator(Elevator elevator)
    {
        elevators.Remove(elevator);
    }

    public Elevator GetClosestElevator(Vector3 position)
    {
        Elevator closest = null;
        float closestDistance = float.MaxValue;

        foreach (Elevator elevator in elevators)
        {
            float distance = Vector3.Distance(elevator.transform.position, position);

            if (distance >= closestDistance)
                continue;

            closestDistance = distance;
            closest = elevator;
        }

        return closest;
    }

    public void NotifyPassengerLifted()
    {
        OnPassengerLifted?.Invoke();
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

        bool completed = await canvas.Ride(request.npc, UnityEngine.Random.Range(2, totalFloors + 1), miniGameLevels);

        canvas.Close();

        if (!completed)
            return;

        SanityManager.Instance.DecreaseSanity(sanityCost);
        request.ride.TrySetResult();
    }
}
