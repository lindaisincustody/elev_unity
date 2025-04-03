using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCanvas : MonoBehaviour
{
    [SerializeField] private AbilityIcon abilityIconPrefab;

    private PlayerAbilities playerAbilities;
    private Dictionary<Ability, AbilityIcon> abilityIcons = new();

    private void Start()
    {   
        playerAbilities = Player.instance.Get<PlayerAbilities>();

        playerAbilities.OnAbilityAdded += OnAbilityAdded;
        playerAbilities.OnAbilityRemoved += OnAbilityRemoved;
    }

    private void OnAbilityAdded(Ability ability)
    {
        AbilityIcon abilityIcon = Instantiate(abilityIconPrefab, transform);
        abilityIcon.Init(ability);
        abilityIcons[ability] = abilityIcon;
    }

    private void OnAbilityRemoved(Ability ability)
    {

    }

    private void OnDestroy()
    {
        playerAbilities.OnAbilityAdded -= OnAbilityAdded;
        playerAbilities.OnAbilityRemoved -= OnAbilityRemoved;
    }
}
