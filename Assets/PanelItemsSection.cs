using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Needed for UI interaction

public class PanelItemsSection : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject itemPrefab;
    public List<ItemData> itemsToDisplay = new List<ItemData>();
    public TextMeshProUGUI descriptionText; // Reference to the description text object in the shop prefab
    private List<GameObject> instantiatedItems = new List<GameObject>();
    private int selectedIndex = 0;
    public InventoryUI inventoryUI;

    void Start()
    {
        PopulatePanel();
        UpdateDescription(selectedIndex); // Update description at start
    }

    void Update()
    {
        int prevIndex = selectedIndex;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedIndex < instantiatedItems.Count - 1) selectedIndex++;
            scrollRect.verticalNormalizedPosition = 1 - ((float)selectedIndex / (instantiatedItems.Count - 1));
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedIndex > 0) selectedIndex--;
            scrollRect.verticalNormalizedPosition = 1 - ((float)selectedIndex / (instantiatedItems.Count - 1));
        }

        HighlightItem(selectedIndex);

        if (prevIndex != selectedIndex) // If selection changed
        {
            HighlightItem(selectedIndex);
            RemoveHighlight(prevIndex);
            UpdateDescription(selectedIndex); // Update the description when the selection changes
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            PurchaseSelectedItem();
        }
    }

    private void PopulatePanel()
    {
        foreach (var itemData in itemsToDisplay)
        {
            var itemInstance = Instantiate(itemPrefab, scrollRect.content);
            instantiatedItems.Add(itemInstance);

            var itemDetails = itemInstance.GetComponent<ItemDetails>();
            if (itemDetails != null)
            {
                itemDetails.itemName = itemData.itemName;
                itemDetails.itemIcon = itemData.itemIcon;
                itemDetails.UpdateUI();
            }
        }
    }

    public void PurchaseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= itemsToDisplay.Count)
            return;

        var selectedItem = itemsToDisplay[selectedIndex];

        // Implement your logic here to check if the player has enough gold or any other conditions
        // For example:
        // if (playerGold >= selectedItem.cost)
        {
            // Deduct gold or perform other actions

            // Call AddItemToInventory on the InventoryUI script
            // You need a reference to the InventoryUI instance. Let's assume it's available as inventoryUI.

            InventoryUI.Instance.AddItemToInventory(selectedItem);

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
        descriptionText.text = itemsToDisplay[index].description; // Set the description text for the currently selected item
    }
}
