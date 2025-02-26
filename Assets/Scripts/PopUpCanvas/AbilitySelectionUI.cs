using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
        // Pause the game
        Time.timeScale = 0f;
        abilitySelectionPanel.SetActive(true);
        PopulateAbilityButtons();
    }

    private void PopulateAbilityButtons()
    {
        // Clear any previous buttons.
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Filter out abilities the player already has if needed...
        List<Ability> filteredAbilities = availableAbilities; // (Your filtering logic)

        // Limit to three random abilities.
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

        // Close UI and resume the game.
        abilitySelectionPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}