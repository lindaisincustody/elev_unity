using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] InputManager playerInput;
    [SerializeField] Inventory inventory;
    [SerializeField] Attributes attributes;
    [Header("Self References")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] TextMeshProUGUI gold;
    [Header("Level References")]
    [SerializeField] TextMeshProUGUI strengthLevel;
    [SerializeField] TextMeshProUGUI intelligenceLevel;
    [SerializeField] TextMeshProUGUI coordinationLevel;
    [SerializeField] TextMeshProUGUI neutralityLevel;
    [Header("Gold Multipliers References")]
    [SerializeField] TextMeshProUGUI strengthGoldMulti;
    [SerializeField] TextMeshProUGUI intelligenceGoldMulti;
    [SerializeField] TextMeshProUGUI coordinationGoldMulti;
    [SerializeField] TextMeshProUGUI neutralityGoldMulti;
    [SerializeField] Slider strengthSlider;
    [SerializeField] Slider intelligenceSlider;
    [SerializeField] Slider cooridnationSlider;
    [SerializeField] Slider neutralitySlider;

    private bool isInventoryOpen = false;

    private void Awake()
    {
        playerInput.OnInventory += OpenInventory;
    }

    public void OpenInventory()
    {
        RefreshUI();
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
    }

    private void RefreshUI()
    {
        UpdateGoldText();
        UpdateLevels();
        UpdateGoldMultipliers();
    }

    private void UpdateGoldText()
    {
        gold.text = inventory.GetGold().ToString();
    }

    private void UpdateLevels()
    {
        strengthLevel.text = PlayerPrefs.GetInt(Constants.PlayerPrefs.StrengthLevel).ToString();
        intelligenceLevel.text = PlayerPrefs.GetInt(Constants.PlayerPrefs.IntelligenceLevel).ToString();
        coordinationLevel.text = PlayerPrefs.GetInt(Constants.PlayerPrefs.CoordinationLevel).ToString();
        neutralityLevel.text = PlayerPrefs.GetInt(Constants.PlayerPrefs.NeutralityLevel).ToString();
    }

    private void UpdateGoldMultipliers()
    {
        strengthGoldMulti.text = MultiplierFormatter(inventory.GetGoldMultiplier(Attribute.Strength));
        intelligenceGoldMulti.text = MultiplierFormatter(inventory.GetGoldMultiplier(Attribute.Intelligence));
        coordinationGoldMulti.text = MultiplierFormatter(inventory.GetGoldMultiplier(Attribute.Coordination));
        neutralityGoldMulti.text = MultiplierFormatter(inventory.GetGoldMultiplier(Attribute.Neutrality));

        strengthSlider.value = attributes.heroStrength / attributes.numberOfEnemies;
        cooridnationSlider.value = attributes.heroIntelligence / attributes.numberOfEnemies;
        intelligenceSlider.value = attributes.heroCoordination/ attributes.numberOfEnemies;
        neutralitySlider.value = attributes.heroNeutrality/ attributes.numberOfEnemies;
    }

    private string MultiplierFormatter(float multiplier)
    {
        return "{ " + multiplier.ToString("0.00") + " X)";
    }

    private void OnDestroy()
    {
        playerInput.OnInventory -= OpenInventory;
    }
}
