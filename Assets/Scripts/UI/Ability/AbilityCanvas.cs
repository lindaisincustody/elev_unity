using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCanvas : MonoBehaviour
{
    [SerializeField] private AbilityIcon abilityIconPrefab;
    [SerializeField] private RectTransform activeAbilitiesContainer;
    [SerializeField] private RectTransform passiveAbilitiesContainer;

    private PlayerAbilities playerAbilities;
    private Dictionary<Ability, AbilityIcon> abilityIcons = new();

    private void Start()
    {   
        playerAbilities = Player.instance.Get<PlayerAbilities>();

        playerAbilities.OnAbilityAdded += OnAbilityAdded;
        playerAbilities.OnAbilityRemoved += OnAbilityRemoved;
        playerAbilities.Init();
    }

    private void OnAbilityAdded(Ability ability)
    {
        var parent = ability.Type == AbilityType.Passive
           ? passiveAbilitiesContainer
           : activeAbilitiesContainer;

        AbilityIcon abilityIcon = Instantiate(abilityIconPrefab, parent);
        abilityIcon.Init(ability);
        abilityIcons[ability] = abilityIcon;
    }

    private void OnAbilityRemoved(Ability ability)
    {
        if (abilityIcons.TryGetValue(ability, out var icon))
        {
            Destroy(icon.gameObject);
            abilityIcons.Remove(ability);
        }
    }

    private void OnDestroy()
    {
        playerAbilities.OnAbilityAdded -= OnAbilityAdded;
        playerAbilities.OnAbilityRemoved -= OnAbilityRemoved;
    }
}
