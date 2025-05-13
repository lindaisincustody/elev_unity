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

    [Header("Self References")] [SerializeField]
    GameObject inventoryPanel;

    [SerializeField] GameObject inventoryBG;
    [SerializeField] TextMeshProUGUI gold;

    [Header("Level References")] [SerializeField]
    TextMeshProUGUI strengthLevel;

    [SerializeField] TextMeshProUGUI intelligenceLevel;
    [SerializeField] TextMeshProUGUI coordinationLevel;
    [SerializeField] TextMeshProUGUI neutralityLevel;

    [Header("Items")] [SerializeField] InventoryItemSlot[] itemSlots;

    [SerializeField] private Volume postProcessingVolume;

    [Header("Effect Display")] [SerializeField]
    private Image itemIcon;

    [SerializeField] private TextMeshProUGUI effectDurationText;

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
        playerMovement = player.Get<PlayerMovement>();
        itemsInventory = player.Get<ItemsInventory>();

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
        if (isInventoryOpen)
            HighlightItem(selectedIndex);
        else
            RemoveHighlight(selectedIndex);
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
            if (slotIndex == selectedIndex)
            {
                HighlightItem(slotIndex);
            }

            slotIndex++;
        }
    }

    public void UseItem()
    {
        if (!isInventoryOpen)
            return;

        Item item = itemSlots[selectedIndex].GetItem();
        if (item == null)
        {
            Debug.Log("No item in the selected slot.");
            return;
        }

        Player.instance.Get<ItemsInventory>().RemoveItem(item);

        itemSlots[selectedIndex].Clear();
        if (itemsInventory.GetAllItems().Count > 0)
        {
            selectedIndex = Mathf.Min(selectedIndex, itemsInventory.GetAllItems().Count - 1);
            HighlightItem(selectedIndex);
        }

        item.Use();
    }

    private void UpdateGoldText()
    {
        gold.text = player.GetGold().ToString();
    }

    public void HighlightItem(int index)
    {
        if (index >= 0 && index < itemSlots.Length)
            itemSlots[index].GetComponent<Image>().color = Color.blue;
    }

    private void RemoveHighlight(int index)
    {
        if (index >= 0 && index < itemSlots.Length)
            itemSlots[index].GetComponent<Image>().color = Color.white;
    }

    private void OnNavigate(Vector2 mousePosition)
    {
        if (!isInventoryOpen) return;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            RectTransform slotRect = itemSlots[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, mousePosition))
            {
                if (selectedIndex != i)
                {
                    RemoveHighlight(selectedIndex);
                    selectedIndex = i;
                    HighlightItem(selectedIndex);
                }

                return;
            }
        }
    }
}