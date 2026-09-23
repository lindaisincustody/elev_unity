using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RoomManager : CoreService
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private List<RoomId> unlockedAtStart = new List<RoomId>();

    private RoomsSnapshot snapshot;

    public event Action<RoomId, bool> OnRoomChanged;

    public override UniTask Initialize()
    {
        Instance = this;

        snapshot = SaveLoadService.Instance.Get<GeneralSaveFile>().RoomsSnapshot;

        if (!snapshot.Seeded)
            Seed();

        return UniTask.CompletedTask;
    }

    public bool IsUnlocked(RoomId id)
    {
        return snapshot.Unlocked.Contains(id);
    }

    public void Unlock(RoomId id)
    {
        Set(id, true);
    }

    public void Lock(RoomId id)
    {
        Set(id, false);
    }

    public void Set(RoomId id, bool unlocked)
    {
        if (IsUnlocked(id) == unlocked)
            return;

        if (unlocked)
            snapshot.Unlocked.Add(id);
        else
            snapshot.Unlocked.Remove(id);

        SaveLoadService.Instance.SaveProgress();

        OnRoomChanged?.Invoke(id, unlocked);
    }

    private void Seed()
    {
        snapshot.Seeded = true;
        snapshot.Unlocked.Clear();
        snapshot.Unlocked.AddRange(unlockedAtStart);

        SaveLoadService.Instance.SaveProgress();
    }
}
