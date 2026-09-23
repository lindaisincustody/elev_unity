using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NpcBrain : Component
{
    [SerializeField] private NpcRoutine routine;
    [SerializeField] private List<NpcReaction> reactions = new List<NpcReaction>();

    public NpcContext Context { get; private set; }

    private CancellationTokenSource routineSource;
    private CancellationToken destroyToken;
    private NpcTask pendingInterrupt;
    private bool paused;

    private Npc npc => (Npc)Entity;

    private void Start()
    {
        destroyToken = gameObject.GetCancellationTokenOnDestroy();
        Context = new NpcContext(npc, this, npc.Get<NpcMovement>());
        routineSource = CancellationTokenSource.CreateLinkedTokenSource(destroyToken);

        Run().Forget();
    }

    private void Update()
    {
        Context.movement.Move();

        if (paused)
            return;

        foreach (NpcReaction reaction in reactions)
        {
            if (!reaction.IsReady(Context))
                continue;

            reaction.MarkTriggered();
            Interrupt(reaction.task);
            return;
        }
    }

    public void Interrupt(NpcTask task)
    {
        pendingInterrupt = task;
        routineSource.Cancel();
    }

    public void Pause()
    {
        paused = true;
        routineSource.Cancel();
    }

    public void Resume()
    {
        paused = false;
    }

    private async UniTaskVoid Run()
    {
        try
        {
            await RunTasks();
        }
        finally
        {
            if (!destroyToken.IsCancellationRequested)
                Context.movement.target = null;
        }
    }

    private async UniTask RunTasks()
    {
        while (!destroyToken.IsCancellationRequested)
        {
            try
            {
                await RunInterrupts();
                await UniTask.WaitWhile(() => paused, cancellationToken: destroyToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            RenewRoutineSource();

            try
            {
                await RunRoutine(routineSource.Token);
            }
            catch (OperationCanceledException)
            {
            }

            if (destroyToken.IsCancellationRequested)
                return;

            Context.movement.target = null;
        }
    }

    private void RenewRoutineSource()
    {
        CancellationTokenSource previous = routineSource;

        routineSource = CancellationTokenSource.CreateLinkedTokenSource(destroyToken);

        previous.Dispose();
    }

    private async UniTask RunInterrupts()
    {
        while (pendingInterrupt != null)
        {
            NpcTask interrupt = pendingInterrupt;
            pendingInterrupt = null;

            await interrupt.Run(Context, destroyToken);
        }
    }

    private async UniTask RunRoutine(CancellationToken token)
    {
        do
        {
            foreach (NpcTask task in routine.tasks)
                await task.Run(Context, token);

            await UniTask.Yield(token);
        }
        while (routine.loop);

        await UniTask.WaitUntil(() => pendingInterrupt != null, cancellationToken: token);
    }
}
