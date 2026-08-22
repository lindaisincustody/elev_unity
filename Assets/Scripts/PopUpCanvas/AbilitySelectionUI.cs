using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class AbilitySelectionUI : MonoBehaviour
{

    [SerializeField] private GameObject abilitySelectionPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private TMP_Text abilityDescriptionText;
    [SerializeField] private AbilitySelectionButton buttonPrefab;

    public List<Ability> availableAbilities;

    private List<AbilityTier> currentTiersToShow;

    private void Start()
    {
        abilitySelectionPanel.SetActive(false);
    }

    public void Show(List<AbilityTier> abilityTiers)
    {
        currentTiersToShow = new List<AbilityTier>(abilityTiers);

        List<Ability> filteredAbilities = GetFilteredAbilities();
        if (filteredAbilities.Count == 0)
            return;

        Time.timeScale = 0f;
        abilitySelectionPanel.SetActive(true);
        PopulateAbilityButtons(filteredAbilities);
    }

    public bool CanProvideAbilityForTier(AbilityTier tier)
    {
        var playerAbilities = Player.instance.GetComponent<PlayerAbilities>().Abilities;
        var available = availableAbilities
                        .Where(a => !playerAbilities.Contains(a))
                        .Select(a => a.Tier)
                        .Distinct()
                        .OrderBy(t => t)
                        .ToList();

        for (int t = (int)tier; t >= (int)AbilityTier.Tier1; t--)
            if (available.Contains((AbilityTier)t))
                return true;

        return false;
    }


    private List<Ability> GetFilteredAbilities()
    {
        var playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
        return availableAbilities
            .Where(ability => !playerAbilities.Abilities.Contains(ability))
            .ToList();
    }

    private void PopulateAbilityButtons(List<Ability> filteredAbilities)
    {
        ClearExistingButtons();

        List<Ability> selectedAbilities = new List<Ability>();

        foreach (var requestedTier in currentTiersToShow)
        {
            Ability picked = null;

            for (int tierLevel = (int)requestedTier; tierLevel >= (int)AbilityTier.Tier1; tierLevel--)
            {
                var options = filteredAbilities
                    .Where(a => (int)a.Tier == tierLevel)
                    .OrderBy(_ => Random.value)
                    .ToList();

                if (options.Count > 0)
                {
                    picked = options[0];
                    break;
                }
            }

            if (picked != null)
            {
                selectedAbilities.Add(picked);
                filteredAbilities.Remove(picked);
            }
        }

        foreach (var ability in selectedAbilities)
            CreateAbilityButton(ability);
    }

    private void ClearExistingButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateAbilityButton(Ability ability)
    {
        AbilitySelectionButton buttonObj = Instantiate(buttonPrefab, buttonContainer);

        buttonObj.AbilityNameText.text = ability.name;
        buttonObj.DescriptionText.text = ability.description;
        buttonObj.TierText.text = ability.Tier.ToString();
        buttonObj.AbilityIcon.sprite = ability.icon;

        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(() => OnAbilitySelected(ability));
    }

    private void OnAbilitySelected(Ability ability)
    {
        var playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
        playerAbilities?.Add(ability);

        abilitySelectionPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
