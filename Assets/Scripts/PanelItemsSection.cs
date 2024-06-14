using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Needed for UI interaction

public class PanelItemsSection : MonoBehaviour
{
    [Header("Self-References")]
    public ShopUI shopUI;
    public Transform parent;
    public ScrollRect scrollRect;
    public GameObject itemPrefab;
    public TextMeshProUGUI descriptionText;
    [Header("Items")]
    public List<ShopItem> shopItems = new List<ShopItem>();
    private List<GameObject> instantiatedItems = new List<GameObject>();
    private int selectedIndex = 0;

    private bool isShopOpen = false;

    private Player player;
    private InputManager playerInput;

    private Coroutine openShop;

    void Start()
    {
        player = Player.instance;
        playerInput = player.GetInputManager;
        playerInput.OnNavigate += OnNavigate;
        playerInput.OnSubmit += Buy;

        PopulatePanel();
        UpdateDescription(selectedIndex); // Update description at start
        HighlightItem(0);
    }

    private void OnDestroy()
    {
        playerInput.OnNavigate -= OnNavigate;
        playerInput.OnSubmit -= Buy;
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
        if (!isShopOpen)
            return;

        PurchaseSelectedItem();
    }

    private void PopulatePanel()
    {
        foreach (var itemData in shopItems)
        {
            var itemInstance = Instantiate(itemPrefab, parent);
            instantiatedItems.Add(itemInstance);

            var itemDetails = itemInstance.GetComponent<ItemDetails>();
            if (itemDetails != null)
            {
                itemDetails.UpdateUI(itemData.cost.ToString(), itemData.name, itemData.sprite);
            }
        }
    }

    public void PurchaseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= shopItems.Count)
            return;

        var selectedItem = shopItems[selectedIndex];

        //if (player.GetGold() < selectedItem.cost)
        //    return;

        ItemsInventory.Instance.AddItem(selectedItem);
        player.AddGold(-selectedItem.cost);
        shopUI.RefreshShopUI();
    }
    
    public void SetShopOpenState(bool isOpen)
    {
        if (isOpen)
            openShop = StartCoroutine(OpenShop());
        else
        {
            if (openShop != null)
                StopCoroutine(openShop);
            isShopOpen = isOpen;
        }
    }

    private IEnumerator OpenShop()
    {
        yield return new WaitForSeconds(1f);
        isShopOpen = true;
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
