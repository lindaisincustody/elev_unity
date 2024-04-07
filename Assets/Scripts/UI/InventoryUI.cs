using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] InputManager playerInput;
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerMovement playerMovement;
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
    [SerializeField] Image strengthSlider;
    [SerializeField] Image intelligenceSlider;
    [SerializeField] Image cooridnationSlider;
    [SerializeField] Image neutralitySlider;

    private bool isInventoryOpen = false;
    private float multiplierRectWidth = 0;
    private float multiplierRectHeight = 0;

    float heroStrength;
    float heroCoordination;
    float heroIntelligence;
    float heroNeutrality;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        multiplierRectWidth = strengthSlider.rectTransform.rect.width * 4;
        multiplierRectHeight = strengthSlider.rectTransform.rect.height;
        playerInput.OnInventory += OpenInventory;
    }

    private void OnDestroy()
    {
        playerInput.OnInventory -= OpenInventory;
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void AddItemToInventory(ItemData itemData)
    {
        // Find the first empty slot in the inventory
        foreach (Transform child in inventoryPanel.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                // Assume there's an Image component to represent the item icon
                var image = child.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = itemData.itemIcon;
                }

                // Optionally, set the name or any other data you need
                // For example, a tooltip or description when hovering over the item

                break; // Exit the loop after filling one slot
            }
        }

        // Update any other UI elements or internal data structures representing the inventory
    }

    public void OpenInventory()
    {
        RefreshUI();
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        playerMovement.SetMovement(!isInventoryOpen);
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

        heroStrength = PlayerPrefs.GetInt(Constants.PlayerPrefs.StrengthMultiplier, 1);
        heroCoordination = PlayerPrefs.GetInt(Constants.PlayerPrefs.CoordinationMultiplier, 1);
        heroIntelligence = PlayerPrefs.GetInt(Constants.PlayerPrefs.IntelligenceMultiplier, 1);
        heroNeutrality = PlayerPrefs.GetInt(Constants.PlayerPrefs.NeutralityMultiplier, 1);

        strengthSlider.rectTransform.sizeDelta = new Vector2(heroStrength / TotalMultiplier() * multiplierRectWidth, multiplierRectHeight);
        cooridnationSlider.rectTransform.sizeDelta = new Vector2(heroCoordination / TotalMultiplier() * multiplierRectWidth, multiplierRectHeight);
        intelligenceSlider.rectTransform.sizeDelta = new Vector2(heroIntelligence / TotalMultiplier() * multiplierRectWidth, multiplierRectHeight);
        neutralitySlider.rectTransform.sizeDelta = new Vector2(heroNeutrality / TotalMultiplier() * multiplierRectWidth, multiplierRectHeight);


        strengthGoldMulti.rectTransform.sizeDelta = new Vector2(heroStrength / TotalMultiplier() * multiplierRectWidth, 9);
        intelligenceGoldMulti.rectTransform.sizeDelta = new Vector2(heroCoordination / TotalMultiplier() * multiplierRectWidth, 9);
        coordinationGoldMulti.rectTransform.sizeDelta = new Vector2(heroIntelligence / TotalMultiplier() * multiplierRectWidth, 9);
        neutralityGoldMulti.rectTransform.sizeDelta = new Vector2(heroNeutrality / TotalMultiplier() * multiplierRectWidth, 9);
    }

    private float TotalMultiplier()
    {
        return heroStrength + heroCoordination + heroIntelligence + heroNeutrality;
    }

    private string MultiplierFormatter(float multiplier)
    {
        return multiplier.ToString("0.00") + " x";
    }

}