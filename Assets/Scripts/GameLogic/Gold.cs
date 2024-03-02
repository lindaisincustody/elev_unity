using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : IDisposable
{
    private float StrengthGoldMultiplier = 1;
    private float CoordinationGoldMultiplier = 1;
    private float IntelligenceGoldMultiplier = 1;
    private float NeutralityGoldMultiplier = 1;

    private int currentGold = 0;

    public Gold()
    {
        ActionManager.OnGoldMultiplierChange += AddMultiplier;
    }

    public void AddGold(int ammount)
    {
        currentGold += ammount;
    }

    public int GetGold()
    {
        return currentGold;
    }

    public void AddMultiplier(Attribute attribute, float multiplier)
    {
        switch (attribute)
        {
            case Attribute.Strength:
                StrengthGoldMultiplier += multiplier;
                break;
            case Attribute.Coordination:
                CoordinationGoldMultiplier += multiplier;
                break;
            case Attribute.Inteliigence:
                IntelligenceGoldMultiplier += multiplier;
                break;
            case Attribute.Neutrality:
                NeutralityGoldMultiplier += multiplier;
                break;
        }
        DebugPanel.Instance.UpdateGoldMultiplier();
    }

    public float GetMultiplier(Attribute attribute)
    {
        switch (attribute)
        {
            case Attribute.Strength:
                return StrengthGoldMultiplier;
            case Attribute.Coordination:
                return CoordinationGoldMultiplier;
            case Attribute.Inteliigence:
                return IntelligenceGoldMultiplier;
            case Attribute.Neutrality:
                return NeutralityGoldMultiplier;
            default:
                return -1;
        }
    }

    public void Dispose()
    {
        ActionManager.OnGoldMultiplierChange -= AddMultiplier;
    }
}

public enum Attribute
{
    Strength,
    Inteliigence,
    Coordination,
    Neutrality
}
