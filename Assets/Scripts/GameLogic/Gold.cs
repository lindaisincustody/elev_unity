using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : IDisposable
{
    private float StrengthGoldMultiplier = 0;
    private float CoordinationGoldMultiplier = 0;
    private float IntelligenceGoldMultiplier = 0;
    private float NeutralityGoldMultiplier = 0;

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
            case Attribute.Intelligence:
                IntelligenceGoldMultiplier += multiplier;
                break;
            case Attribute.Neutrality:
                NeutralityGoldMultiplier += multiplier;
                break;
        }
    }

    public float GetMultiplier(Attribute attribute)
    {
        switch (attribute)
        {
            case Attribute.Strength:
                return StrengthGoldMultiplier;
            case Attribute.Coordination:
                return CoordinationGoldMultiplier;
            case Attribute.Intelligence:
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
    Intelligence,
    Coordination,
    Neutrality
}
