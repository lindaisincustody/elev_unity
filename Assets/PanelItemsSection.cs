using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Needed for UI interaction

public class PanelItemsSection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] InputManager playerInput;
    [Header("Self-References")]
    public ScrollRect scrollRect;
    public GameObject itemPrefab;
    public TextMeshProUGUI descriptionText;
    [Header("Items")]
    public List<ShopItem> shopItems = new List<ShopItem>();
    private List<GameObject> instantiatedItems = new List<GameObject>();
    private int selectedIndex = 0;

    private void Awake()
    {
        playerInput.OnNavigate += OnNavigate;
        playerInput.OnSubmit += Buy;
    }

    void Start()
    {
        PopulatePanel();
        UpdateDescription(selectedIndex); // Update description at start
        HighlightItem(0);
    }

    private void OnNavigate(Vector2 value)
    {
        //if (!isShopActive)
        //   return;

        int prevIndex = selectedIndex;

        if (value == Vector2.up)
        {
            if (selectedIndex > 0) selectedIndex--;
            scrollRect.verticalNormalizedPosition = 1 - ((float)selectedIndex / (instantiatedItems.Count - 1));
        }
        else if (value == Vector2.down)
        {
            if (selectedIndex < instantiatedItems.Count - 1) selectedIndex++;
            scrollRect.verticalNormalizedPosition = 1 - ((float)selectedIndex / (instantiatedItems.Count - 1));
        }

        HighlightItem(selectedIndex);

        if (prevIndex != selectedIndex)
        {
            HighlightItem(selectedIndex);
            RemoveHighlight(prevIndex);
            UpdateDescription(selectedIndex);
        }
    }

    private void Buy()
    {
        PurchaseSelectedItem();
    }

    private void PopulatePanel()
    {
        foreach (var itemData in shopItems)
        {
            var itemInstance = Instantiate(itemPrefab, scrollRect.content);
            instantiatedItems.Add(itemInstance);

            var itemDetails = itemInstance.GetComponent<ItemDetails>();
            if (itemDetails != null)
            {
                itemDetails.itemName = itemData.itemName;
                itemDetails.description = itemData.description;
                itemDetails.itemIcon = itemData.sprite;
                itemDetails.UpdateUI();
            }
        }
    }

    public void PurchaseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= shopItems.Count)
            return;

        var selectedItem = shopItems[selectedIndex];

        // Implement your logic here to check if the player has enough gold or any other conditions
        // For example:
        // if (playerGold >= selectedItem.cost)
        {
            // Deduct gold or perform other actions

            // Call AddItemToInventory on the InventoryUI script
            // You need a reference to the InventoryUI instance. Let's assume it's available as inventoryUI.
            ItemsInventory.Instance.AddItem(selectedItem);

            // Optional: Remove the item from the shop or update the shop UI
        }
    }

    private void HighlightItem(int index)
    {
        instantiatedItems[index].GetComponentInChildren<Image>().color = new Color(0, 0, 1, 0.2f);

    }

    private void RemoveHighlight(int index)
    {

        instantiatedItems[index].GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f); // Semi-transparent white

    }
    private void UpdateDescription(int index)
    {
        descriptionText.text = shopItems[index].description; // Set the description text for the currently selected item
    }
}
