using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class CoreStartup : MonoBehaviour
{
    [Serializable]
    private class Stage
    {
        public StartupState state;
        public List<CoreService> services = new List<CoreService>();
    }

    public static CoreStartup Instance { get; private set; }

    [SerializeField] private List<Stage> stages = new List<Stage>();

    public StartupState State { get; private set; }

    public event Action<StartupState> OnStateChanged;

    private void Awake()
    {
        Instance = this;
        Run().Forget();
    }

    private async UniTaskVoid Run()
    {
        foreach (Stage stage in stages)
        {
            SetState(stage.state);

            foreach (CoreService service in stage.services)
                await service.Initialize();
        }

        SetState(StartupState.Ready);
    }

    private void SetState(StartupState state)
    {
        State = state;
        OnStateChanged?.Invoke(state);
    }
}

public enum StartupState
{
    None,
    Config,
    Save,
    Managers,
    UI,
    Spawners,
    Ready
}
