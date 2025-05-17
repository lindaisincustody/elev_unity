using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
using UnityEngine.InputSystem;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }

    [SerializeField]
    private CraftingRecipes craftingRecipes;

    [SerializeField]
    private GameObject craftingPanel;

    [SerializeField]
    private GameObject craftingBG;

    [SerializeField]
    private InventoryItemSlot[] itemSlots;

    [SerializeField]
    private InventoryItemSlot[] craftSlots;

    private Player player;
    private ItemsInventory itemsInventory;
    private DataManager dataManager;
    private PlayerMovement playerMovement;
    private InputManager playerInput;

    private bool isCraftingOpen = false;
    private const int numberOfColumns = 4;

    public float savedDuration = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
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
        playerMovement = player.Get<PlayerMovement>();
        itemsInventory = player.Get<ItemsInventory>();
        playerInput = player.GetInputManager;

        playerInput.OnUICancel += ClosePanel;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (isCraftingOpen)
        {
            ReturnCraftItemsToInventory();
        }

        playerInput.OnUICancel -= ClosePanel;
    }

    [Button]
    public void ToggleCrafting()
    {
        if (isCraftingOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        if (isCraftingOpen)
        {
            return;
        }

        RefreshUI();
        InventoryUI.Instance.CanOpenInventory(false);
        isCraftingOpen = true;
        craftingPanel.SetActive(true);
        craftingBG.SetActive(true);
        playerMovement.SetMovement(false);
    }

    public void ClosePanel()
    {
        if (!isCraftingOpen)
        {
            return;
        }

        ReturnCraftItemsToInventory();
        InventoryUI.Instance.CanOpenInventory(true);
        isCraftingOpen = false;
        craftingPanel.SetActive(false);
        craftingBG.SetActive(false);
        playerMovement.SetMovement(true);
    }

    public void Craft()
    {
        Item craftedItem = craftingRecipes.TryToCraft(craftSlots);
        if (craftedItem == null)
            return;

        itemsInventory.AddItem(craftedItem);
        for (int i = 0; i < craftSlots.Length; i++)
        {
            InventoryItemSlot slot = craftSlots[i];
            if (slot.Item != null)
            {
                slot.Clear();
            }
        }

        RefreshUI();
        Debug.Log(craftedItem.name);
    }

    private void RefreshUI()
    {
        UpdateItemSlots();
        UpdateCraftSlots();
    }

    private void UpdateItemSlots()
    {
        List<Item> inventoryItems = itemsInventory.GetAllItems();
        List<Item> shardItems = inventoryItems.Where(i => i is AbilityShardItem).ToList();
        List<IGrouping<string, Item>> groupedShards = shardItems.GroupBy(i => i.itemId).ToList();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Clear();
            itemSlots[i].OnClick = null;
        }

        int slotIndex = 0;

        foreach (IGrouping<string, Item> group in groupedShards)
        {
            if (slotIndex >= itemSlots.Length)
            {
                Debug.LogError("Too many unique items for available slots!");
                break;
            }

            AbilityShardItem shard = group.First() as AbilityShardItem;
            int count = group.Count();

            InventoryItemSlot slot = itemSlots[slotIndex];
            slot.Equip(shard, count);
            slot.slotIndex = slotIndex;
            slot.OnClick += MoveToCraft;

            slotIndex++;
        }
    }

    private void UpdateCraftSlots()
    {
        for (int i = 0; i < craftSlots.Length; i++)
        {
            InventoryItemSlot slot = craftSlots[i];
            slot.OnClick = null;
            Item current = slot.Item;
            if (current is AbilityShardItem)
            {
                slot.slotIndex = i;
                slot.OnClick += MoveToInventory;
            }
        }
    }

    private void MoveToCraft(int itemIndex)
    {
        AbilityShardItem shard = itemSlots[itemIndex].Item as AbilityShardItem;
        if (shard == null)
        {
            return;
        }

        int emptyIndex = -1;
        for (int i = 0; i < craftSlots.Length; i++)
        {
            if (craftSlots[i].Item == null)
            {
                emptyIndex = i;
                break;
            }
        }

        if (emptyIndex < 0)
        {
            return;
        }

        InventoryItemSlot craftSlot = craftSlots[emptyIndex];
        craftSlot.Equip(shard, 1);
        craftSlot.slotIndex = emptyIndex;
        craftSlot.OnClick += MoveToInventory;

        itemsInventory.RemoveItem(shard);

        int remaining = itemsInventory.GetAllItems().Count(i => i.itemId == shard.itemId);
        if (remaining > 0)
        {
            itemSlots[itemIndex].Equip(shard, remaining);
        }
        else
        {
            RefreshUI();
        }
    }

    private void MoveToInventory(int craftIndex)
    {
        AbilityShardItem shard = craftSlots[craftIndex].Item as AbilityShardItem;
        if (shard == null)
        {
            return;
        }

        craftSlots[craftIndex].Clear();
        itemsInventory.AddItem(shard);
        RefreshUI();
    }

    private void ReturnCraftItemsToInventory()
    {
        for (int i = 0; i < craftSlots.Length; i++)
        {
            InventoryItemSlot slot = craftSlots[i];
            AbilityShardItem shard = slot.Item as AbilityShardItem;
            if (shard != null)
            {
                itemsInventory.AddItem(shard);
                slot.Clear();
            }
        }
    }
}
