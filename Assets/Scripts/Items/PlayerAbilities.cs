using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerAbilitiesData
{
    public List<string> abilityIds = new List<string>();
}

public class PlayerAbilities : Component
{
    public List<Ability> Abilities = new();
    private SavingWrapper savingWrapper;

    public Action<Ability> OnAbilityAdded;
    public Action<Ability> OnAbilityRemoved;

    private void Start()
    {
        savingWrapper = SavingWrapper.Instance;
        LoadAbilities();
        Init();
    }

    public void Init()
    {
        foreach (Ability ability in Abilities)
        {
            AbilityHolder abilityHolder = Player.instance.gameObject.AddComponent<AbilityHolder>();
            abilityHolder.ability = ability;
            abilityHolder.key = ability.KeyCode;
            OnAbilityAdded?.Invoke(ability);
            ability.Start();
        }
    }

    public void Add(Ability ability)
    {
        if (!Abilities.Contains(ability))
        {
            Abilities.Add(ability);
            AbilityHolder abilityHolder = Player.instance.gameObject.AddComponent<AbilityHolder>();
            abilityHolder.ability = ability;
            abilityHolder.key = ability.KeyCode;
            ability.Start();
            OnAbilityAdded?.Invoke(ability);
            SaveAbilities();
        }
    }

    public void Remove(Ability ability)
    {
        if (Abilities.Contains(ability))
        {
            ability.Destroy();
            Abilities.Remove(ability);
            OnAbilityRemoved?.Invoke(ability);
            SaveAbilities();
        }
    }

    public void RemoveAll()
    {
        foreach (Ability ability in Abilities.ToList())
        {
            Remove(ability);
        }
    }

    private void SaveAbilities()
    {
        savingWrapper.SavePlayerAbilities(Abilities);
    }

    private void LoadAbilities()
    {
        Abilities = savingWrapper.LoadPlayerAbilities();
    }
}