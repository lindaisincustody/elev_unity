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

    [Header("Items")]
    [SerializeField] InventoryItemSlot[] itemSlots;

    [SerializeField] private Volume postProcessingVolume;

    [Header("Effect Display")]
    [SerializeField] private Image itemIcon;  // For displaying the item icon
    [SerializeField] private TextMeshProUGUI effectDurationText;  // For displaying the duration of the effect

    Player player;
    ItemsInventory itemsInventory;
    DataManager dataManager;
    InputManager playerInput;
    PlayerMovement playerMovement;

    private bool isInventoryOpen = false;
    private bool canOpenInventory = true;

    private int selectedIndex = 0;
    private int numberOfColumns = 4;

    public float savedDuration = 0;

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
    }

    private void Start()
    {
        player = Player.instance;
        dataManager = DataManager.Instance;
        playerInput = player.GetInputManager;
        playerMovement = player.GetPlayerMovement;
        itemsInventory = player.ItemsInventory;

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
        PopulateItems();
        HighlightItem(0);
    }

    public void PopulateItems()
    {
        var allItems = itemsInventory.GetAllItems();
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

        Item item = itemSlots[selectedIndex].GetItem(); // Retrieve the item from the selected slot
        if (item == null)
        {
            Debug.Log("No item in the selected slot.");
            return;
        }

        Player.instance.ItemsInventory.RemoveItem(item); // Remove the item from the inventory

        itemSlots[selectedIndex].Clear(); // Clear the slot after removing the item
        if (itemsInventory.GetAllItems().Count > 0)
        {
            selectedIndex = Mathf.Min(selectedIndex, itemsInventory.GetAllItems().Count - 1);
            HighlightItem(selectedIndex);
        }
        else
        {
            //inventoryPanel.SetActive(false); 
        }

        player.PlayerAbilities.Add(item.ability);
    }


    private void UpdateGoldText()
    {
        gold.text = player.GetGold().ToString();
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
        int totalItems = itemsInventory.GetAllItems().Count; // Assuming you only want to count filled slots.

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