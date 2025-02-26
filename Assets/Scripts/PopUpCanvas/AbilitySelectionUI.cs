using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class AbilitySelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject abilitySelectionPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject abilityButtonPrefab;
    [SerializeField] private TMP_Text abilityDescriptionText;

    public List<Ability> availableAbilities;

    private void Start()
    {
        abilitySelectionPanel.SetActive(false);
    }

    public void Show()
    {
        Time.timeScale = 0f;
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

        List<Ability> filteredAbilities =
            availableAbilities.FindAll(ability => !playerAbilities.Abilities.Contains(ability));

        int countToShow = Mathf.Min(3, filteredAbilities.Count);
        List<Ability> abilitiesToDisplay = filteredAbilities
            .OrderBy(a => UnityEngine.Random.value)
            .Take(countToShow)
            .ToList();

        foreach (Ability ability in abilitiesToDisplay)
        {
            GameObject btnObj = Instantiate(abilityButtonPrefab, buttonContainer);

            TMP_Text btnText = btnObj.transform.Find("AbilityNameText").GetComponent<TMP_Text>();
            if (btnText != null)
                btnText.text = ability.name;

            TMP_Text descriptionText = btnObj.transform.Find("DescriptionText").GetComponent<TMP_Text>();
            if (descriptionText != null)
                descriptionText.text = ability.description;

            Image abilityIcon = btnObj.transform.Find("AbilityIcon").GetComponent<Image>();
            if (abilityIcon != null)
            {
                abilityIcon.sprite = ability.icon;
                abilityIcon.enabled = (ability.icon != null);
            }

            Button button = btnObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnAbilitySelected(ability));
        }
    }


    private void UpdateDescription(string description)
    {
        abilityDescriptionText.text = description;
    }

    private void OnAbilitySelected(Ability ability)
    {
        PlayerAbilities playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
        if (playerAbilities != null)
        {
            playerAbilities.Add(ability);
        }

        abilitySelectionPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}