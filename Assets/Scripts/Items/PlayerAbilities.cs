using System.Collections;
using System.Collections.Generic;
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
    }

    public void Init()
    {
        foreach (Ability ability in Abilities)
        {
            Debug.Log(ability.name);
        }
    }

    public void Add(Ability ability)
    {
        if (!Abilities.Contains(ability))
        {
            Abilities.Add(ability);
            SaveAbilities();
            Init();
        }
    }

    public void Remove(Ability ability)
    {
        if (Abilities.Contains(ability))
        {
            Abilities.Remove(ability);
            SaveAbilities();
            Init();
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
