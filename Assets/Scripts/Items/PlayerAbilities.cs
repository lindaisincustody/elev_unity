using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PlayerAbilities : Component
{
    public List<Ability> Abilities = new();
    private GeneralSaveFile saveFile;

    public Action<Ability> OnAbilityAdded;
    public Action<Ability> OnAbilityRemoved;

    private bool loadFromResources = true;
    private List<Ability> allAbilities = new();


    public void Start()
    {
        //RemoveAll();
        //AddAllAbilities();
    }
    private void AddAllAbilities()
    {
        IEnumerable<Ability> source = loadFromResources
            ? Resources.LoadAll<Ability>("Abilities")
            : allAbilities;

        foreach (var ability in source)
            Add(ability);

        Debug.Log($"Added {source.Count()} abilities.");
    }


    private void Awake()
    {
        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
    }

    public void Init()
    {
        LoadAbilities();

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

    [Button]
    public void RemoveAll()
    {
        foreach (Ability ability in Abilities.ToList())
        {
            Remove(ability);
        }
    }

    private void SaveAbilities()
    {
        List<string> abilityIds = saveFile.AbilitiesSnapshot.AbilityIds;
        abilityIds.Clear();

        foreach (Ability ability in Abilities)
            abilityIds.Add(ability.abilityId);

        SaveLoadService.Instance.SaveProgress();
    }

    private void LoadAbilities()
    {
        Ability[] resourceAbilities = Resources.LoadAll<Ability>("Abilities");

        Abilities.Clear();

        foreach (string abilityId in saveFile.AbilitiesSnapshot.AbilityIds)
        {
            Ability ability = Array.Find(resourceAbilities, a => a.abilityId == abilityId);

            if (ability != null)
                Abilities.Add(ability);
            else
                Debug.LogWarning("Ability with ID " + abilityId + " not found in Resources.");
        }
    }
}