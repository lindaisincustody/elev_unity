using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AbilityActionRegistry : CoreService
{
    public static AbilityActionRegistry Instance { get; private set; }

    private Dictionary<string, Action> abilityActions = new Dictionary<string, Action>();

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public void RegisterAction(string label, Action action)
    {
        if (!abilityActions.ContainsKey(label))
        {
            abilityActions.Add(label, action);
        }
        else
        {
            abilityActions[label] = action;
        }
    }

    public void ExecuteAction(string label)
    {
        if (abilityActions.TryGetValue(label, out Action action))
        {
            action?.Invoke();
        }
        else
        {
            Debug.Log($"No action registered for label: {label}");
        }
    }
}