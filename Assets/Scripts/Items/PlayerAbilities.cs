using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerAbilitiesData
{
    public List<string> abilityIds = new List<string>();
}

public class PlayerAbilities : MonoBehaviour
{
    public List<Ability> Abilities = new();
    private SavingWrapper savingWrapper;

    private void Start()
    {
        savingWrapper = SavingWrapper.Instance;
        LoadAbilities();
        Init();
        RemoveAll();
    }

    public void Init()
    {
        foreach (Ability ability in Abilities)
        {
            AbilityHolder abilityHolder = Player.instance.gameObject.AddComponent<AbilityHolder>();
            abilityHolder.ability = ability;
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
            ability.Start();
            SaveAbilities();
        }
    }

    public void Remove(Ability ability)
    {
        if (Abilities.Contains(ability))
        {
            ability.Destroy();
            Abilities.Remove(ability);
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
