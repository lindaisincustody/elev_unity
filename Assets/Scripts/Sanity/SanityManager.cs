using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SanityManager : CoreService
{
    public static SanityManager Instance { get; private set; }

    [SerializeField] private int startingSanity = 300;
    [SerializeField] private int maxSanity = 300;
    [SerializeField] private int underworldThreshold = 100;
    [SerializeField] private int tweenSpeed = 150;

    public int CurrentSanity { get; private set; }
    public int MaxSanity => maxSanity;
    public bool IsPlayerInUnderworld { get; private set; }

    public event Action<int> OnSanityChanged;
    public event Action OnWorldChange;

    public override UniTask Initialize()
    {
        Instance = this;

        CurrentSanity = Mathf.Clamp(startingSanity, 0, maxSanity);
        IsPlayerInUnderworld = CurrentSanity < underworldThreshold;

        return UniTask.CompletedTask;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            DecreaseSanity(50);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            AddSanity(50);
    }

    public void AddSanity(int amount)
    {
        SetSanity(CurrentSanity + amount);
    }

    public void DecreaseSanity(int amount)
    {
        SetSanity(CurrentSanity - amount);
    }

    public void SanityToMin()
    {
        DOVirtual.Int(CurrentSanity, 0, tweenSpeed, SetSanity).SetSpeedBased(true);
    }

    public void SanityToMax()
    {
        DOVirtual.Int(CurrentSanity, maxSanity, tweenSpeed, SetSanity).SetSpeedBased(true);
    }

    private void SetSanity(int value)
    {
        CurrentSanity = Mathf.Clamp(value, 0, maxSanity);

        bool inUnderworld = CurrentSanity < underworldThreshold;

        if (inUnderworld != IsPlayerInUnderworld)
        {
            IsPlayerInUnderworld = inUnderworld;
            OnWorldChange?.Invoke();
        }

        OnSanityChanged?.Invoke(CurrentSanity);
    }
}
