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
