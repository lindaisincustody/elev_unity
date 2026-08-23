using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private GameObject inventoryBG;

    [SerializeField]
    private TextMeshProUGUI gold;

    [SerializeField]
    private InventoryItemSlot[] itemSlots;

    [SerializeField]
    private UnityEngine.Rendering.Volume postProcessingVolume;

    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TextMeshProUGUI effectDurationText;

    private Player player;
    private ItemsInventory itemsInventory;
    private DataManager dataManager;
    private InputManager playerInput;
    private PlayerMovement playerMovement;

    private bool isInventoryOpen = false;

    private int selectedIndex = 0;
    private const int numberOfColumns = 4;

    private void Start()
    {
        player = Player.instance;
        dataManager = DataManager.Instance;
        playerInput = InputManager.Instance;
        playerMovement = player.Get<PlayerMovement>();
        itemsInventory = player.Get<ItemsInventory>();

        playerInput.OnInventory += ToggleInventory;
    }

    private void OnDestroy()
    {
        playerInput.OnInventory -= ToggleInventory;

    }

    public void ToggleInventory()
    {
        if (isInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        if (isInventoryOpen)
        {
            return;
        }

        if (!UIManager.Instance.RequestOpen(this))
        {
            return;
        }

        RefreshUI();
        isInventoryOpen = true;
        inventoryPanel.SetActive(true);
        inventoryBG.SetActive(true);
        playerMovement.SetMovement(false);
    }

    public void CloseInventory()
    {
        if (!isInventoryOpen)
        {
            return;
        }

        isInventoryOpen = false;
        inventoryPanel.SetActive(false);
        inventoryBG.SetActive(false);
        playerMovement.SetMovement(true);
        UIManager.Instance.NotifyClosed(this);
    }

    private void RefreshUI()
    {
        UpdateGoldText();
        PopulateItems();
    }

    private class ItemCountPair
    {
        public Item Item;
        public int Count;
    }

    public void PopulateItems()
    {
        List<Item> allItems = itemsInventory.GetAllItems();

        List<ItemCountPair> groupedItems = allItems
            .GroupBy(i => i.itemId)
            .Select(g => new ItemCountPair { Item = g.First(), Count = g.Count() })
            .ToList();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Clear();
            itemSlots[i].OnClick = null;
        }

        int slotIndex = 0;

        foreach (ItemCountPair entry in groupedItems)
        {
            if (slotIndex >= itemSlots.Length)
            {
                Debug.LogError("Too many unique items for available slots!");
                break;
            }

            InventoryItemSlot slot = itemSlots[slotIndex];
            slot.Equip(entry.Item, entry.Count);
            slot.slotIndex = slotIndex;
            slot.OnClick += UseItem;

            slotIndex++;
        }

        if (slotIndex == 0)
        {
            selectedIndex = 0;
        }
    }

    public void UseItem(int index)
    {
        if (!isInventoryOpen)
        {
            return;
        }

        Item item = itemSlots[index].Item;

        if (item == null || item is AbilityShardItem)
        {
            return;
        }

        int beforeCount = itemsInventory
            .GetAllItems()
            .Count(i => i.itemId == item.itemId);

        item.Use();
        itemsInventory.RemoveItem(item);

        if (beforeCount > 1)
        {
            itemSlots[index].Equip(item, beforeCount - 1);
        }
        else
        {
            itemSlots[index].Clear();
            PopulateItems();

            int groupCount = itemsInventory
                .GetAllItems()
                .GroupBy(i => i.itemId)
                .Count();

            selectedIndex = groupCount > 0
                ? Mathf.Clamp(selectedIndex, 0, groupCount - 1)
                : 0;
        }
    }

    private void UpdateGoldText()
    {
        gold.text = player.GetGold().ToString();
    }
}
