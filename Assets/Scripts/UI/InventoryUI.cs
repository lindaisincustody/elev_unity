using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Self References")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject inventoryBG;
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
    [Header("Items")]
    [SerializeField] InventoryItemSlot[] itemSlots;

    [SerializeField] private Volume postProcessingVolume;

    [Header("Effect Display")]
    [SerializeField] private Image itemIcon;  // For displaying the item icon
    [SerializeField] private TextMeshProUGUI effectDurationText;  // For displaying the duration of the effect

    Player player;
    DataManager dataManager;
    InputManager playerInput;
    PlayerMovement playerMovement;

    private bool isInventoryOpen = false;
    private bool canOpenInventory = true;
    private float multiplierRectWidth = 0;
    private float multiplierRectHeight = 0;

    float heroStrength;
    float heroCoordination;
    float heroIntelligence;
    float heroNeutrality;

    private int selectedIndex = 0;
    private int numberOfColumns = 4;

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
    }

    private void Start()
    {
        player = Player.instance;
        dataManager = DataManager.Instance;
        playerInput = player.GetInputManager;
        playerMovement = player.GetPlayerMovement;

        playerInput.OnNavigate += OnNavigate;
        playerInput.OnSubmit += UseItem;
        playerInput.OnInventory += OpenInventory;
    }

    private void OnDestroy()
    {
        playerInput.OnNavigate -= OnNavigate;
        playerInput.OnSubmit -= UseItem;
        playerInput.OnInventory -= OpenInventory;
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void OpenInventory()
    {
        if (!canOpenInventory)
            return;

        RefreshUI();
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        inventoryBG.SetActive(isInventoryOpen);
        playerMovement.SetMovement(!isInventoryOpen);
        if(isInventoryOpen)
        HighlightItem(selectedIndex);
        else RemoveHighlight(selectedIndex);
    }

    public void CanOpenInventory(bool canOpen)
    {
        canOpenInventory = canOpen;
    }

    private void RefreshUI()
    {
        UpdateGoldText();
        UpdateLevels();
        UpdateGoldMultipliers();
        PopulateItems();
        HighlightItem(0);
    }

    public void PopulateItems()
    {
        var allItems = ItemsInventory.Instance.GetAllItems();
        int slotIndex = 0;
        foreach (var item in allItems)
        {
            if (itemSlots.Length == slotIndex)
            {
                Debug.LogError("Too many items, not enough slots to display");
                break;
            }
            itemSlots[slotIndex].Equip(item);
            if (slotIndex == selectedIndex) // Highlight the selected index item
            {
                HighlightItem(slotIndex);
            }
            slotIndex++;
        }
    }

    private void UseItem()
    {
        if (!isInventoryOpen)
            return;

        ShopItem item = itemSlots[selectedIndex].GetItem(); // Retrieve the item from the selected slot
        if (item == null)
        {
            Debug.Log("No item in the selected slot.");
            return;
        }

        itemIcon.sprite = item.sprite;  // Assuming each ShopItem has a Sprite property 'Icon'
        itemIcon.gameObject.SetActive(true);  // Ensure the icon is visible

        ItemsInventory.Instance.RemoveItem(item); // Remove the item from the inventory

        itemSlots[selectedIndex].Clear(); // Clear the slot after removing the item
        if (ItemsInventory.Instance.GetAllItems().Count > 0)
        {
            selectedIndex = Mathf.Min(selectedIndex, ItemsInventory.Instance.GetAllItems().Count - 1);
            HighlightItem(selectedIndex);
        }
        else
        {
            //inventoryPanel.SetActive(false); 
        }
        StartCoroutine(ApplyTrippyEffects());
    }

    private IEnumerator ApplyTrippyEffects()
    {
        float duration = 60f;  // Total duration of the effect
        float elapsed = 0f;

        ChromaticAberration chromaticAberration = null;
        LensDistortion lensDistortion = null;

        if (postProcessingVolume.profile.TryGet(out chromaticAberration) &&
            postProcessingVolume.profile.TryGet(out lensDistortion))
        {
            float maxChromaticIntensity = 1f; // Maximum intensity for Chromatic Aberration
            float minLensDistortion = -1f; // Minimum intensity for Lens Distortion
            float maxLensDistortion = 1f; // Maximum intensity for Lens Distortion
            float frequencyMultiplier = 5f; // Oscillation frequency

            while (elapsed < duration)
            {
                float remainingTime = duration - elapsed;
                effectDurationText.text = $"Effect Duration: {remainingTime.ToString("0.0")}s";  // Update the duration text

                float sinusoidalFactor = Mathf.Sin(2 * Mathf.PI * frequencyMultiplier * elapsed / duration);
                chromaticAberration.intensity.value = (sinusoidalFactor + 1f) / 2 * maxChromaticIntensity;
                lensDistortion.intensity.value = sinusoidalFactor * (maxLensDistortion - minLensDistortion) / 2 + (maxLensDistortion + minLensDistortion) / 2;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Reset effects and UI
            chromaticAberration.intensity.value = 0f;
            lensDistortion.intensity.value = 0f;
            effectDurationText.text = "Effect Duration: 0.0s";
            itemIcon.gameObject.SetActive(false);  // Hide the item icon
        }
    }


    private void UpdateGoldText()
    {
        gold.text = player.GetGold().ToString();
    }

    private void UpdateLevels()
    {
        strengthLevel.text = dataManager.GetLevel(Attribute.Strength).ToString();
        intelligenceLevel.text = dataManager.GetLevel(Attribute.Intelligence).ToString();
        coordinationLevel.text = dataManager.GetLevel(Attribute.Coordination).ToString();
        neutralityLevel.text = dataManager.GetLevel(Attribute.Neutrality).ToString();
    }

    private void UpdateGoldMultipliers()
    {
        strengthGoldMulti.text = MultiplierFormatter(player.GetGoldMultiplier(Attribute.Strength));
        intelligenceGoldMulti.text = MultiplierFormatter(player.GetGoldMultiplier(Attribute.Intelligence));
        coordinationGoldMulti.text = MultiplierFormatter(player.GetGoldMultiplier(Attribute.Coordination));
        neutralityGoldMulti.text = MultiplierFormatter(player.GetGoldMultiplier(Attribute.Neutrality));

        heroStrength = player.GetGoldMultiplier(Attribute.Strength);
        heroCoordination = player.GetGoldMultiplier(Attribute.Coordination);
        heroIntelligence = player.GetGoldMultiplier(Attribute.Intelligence);
        heroNeutrality = player.GetGoldMultiplier(Attribute.Neutrality);

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

    private void HighlightItem(int index)
    {
        if (index >= 0 && index < itemSlots.Length)
            itemSlots[index].GetComponent<Image>().color = Color.blue; // Example color
    }

    private void RemoveHighlight(int index)
    {
        if (index >= 0 && index < itemSlots.Length)
            itemSlots[index].GetComponent<Image>().color = Color.white; // Default color
    }

    private void OnNavigate(Vector2 direction)
    {
        if (!isInventoryOpen) return;

        int prevIndex = selectedIndex;
        int totalItems = ItemsInventory.Instance.GetAllItems().Count; // Assuming you only want to count filled slots.

        if (direction.y > 0) // Up
        {
            if (selectedIndex >= numberOfColumns) // Move up a row
                selectedIndex -= numberOfColumns;
            else
                selectedIndex = ((totalItems - 1) / numberOfColumns) * numberOfColumns + (selectedIndex % numberOfColumns); // Wrap to the bottom
        }
        else if (direction.y < 0) // Down
        {
            if (selectedIndex + numberOfColumns < totalItems) // Move down a row
                selectedIndex += numberOfColumns;
            else
                selectedIndex = selectedIndex % numberOfColumns; // Wrap to the top
        }

        if (direction.x > 0) // Right
        {
            selectedIndex++;
            if (selectedIndex >= totalItems) // Wrap to the first item
                selectedIndex = 0;
        }
        else if (direction.x < 0) // Left
        {
            if (selectedIndex == 0) // Wrap to the last item
                selectedIndex = totalItems - 1;
            else
                selectedIndex--;
        }

        if (prevIndex != selectedIndex)
        {
            RemoveHighlight(prevIndex);
            HighlightItem(selectedIndex);
        }
    }

}
