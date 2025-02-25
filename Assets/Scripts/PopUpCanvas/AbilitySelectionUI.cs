using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject abilitySelectionPanel; 
    [SerializeField] private Transform buttonContainer; 
    [SerializeField] private GameObject abilityButtonPrefab; 

    public List<Ability> availableAbilities;

    private void Start()
    {

        abilitySelectionPanel.SetActive(false);
    }

    public void Show()
    {
        abilitySelectionPanel.SetActive(true);
        PopulateAbilityButtons();
    }

    private void PopulateAbilityButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        PlayerAbilities playerAbilities = Player.instance.GetComponent<PlayerAbilities>();

        List<Ability> filteredAbilities = availableAbilities.FindAll(ability =>
            !playerAbilities.Abilities.Contains(ability));

        int countToShow = Mathf.Min(3, filteredAbilities.Count);

        List<Ability> abilitiesToDisplay = filteredAbilities
            .OrderBy(a => UnityEngine.Random.value)
            .Take(countToShow)
            .ToList();

        foreach (Ability ability in abilitiesToDisplay)
        {

            GameObject btnObj = Instantiate(abilityButtonPrefab, buttonContainer);

            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            btnText.text = ability.name;

            Button button = btnObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnAbilitySelected(ability));
        }
    }


    private void OnAbilitySelected(Ability ability)
    {

        PlayerAbilities playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
        if (playerAbilities != null)
        {
            playerAbilities.Add(ability);
        }

        abilitySelectionPanel.SetActive(false);
    }
}
