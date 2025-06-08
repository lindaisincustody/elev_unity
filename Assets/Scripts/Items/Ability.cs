using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability : ScriptableObject
{
    public KeyCode KeyCode;
    public AbilityType Type;
    public AbilityTier Tier;

    public Action OnActivate;
    public Action OnCooldown;

    public Sprite icon;

    public string abilityId;
    public new string name;
    public float cooldownTime;
    public float activeTime;

    public string description;

    public virtual void Activate()
    {
        OnActivate?.Invoke();
    }

    public virtual void End()
    {
        OnCooldown?.Invoke();
    }

    public virtual void CooldownEnd()
    {
    }

    public virtual void Start()
    {
    }

    public virtual void Destroy()
    {
    }
}

public enum AbilityType
{
    Active,
    Passive
}

public enum AbilityTier
{
    Tier1,
    Tier2,
    Tier3
}