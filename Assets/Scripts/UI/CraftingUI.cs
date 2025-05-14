using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NaughtyAttributes;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }

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
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
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

        isCraftingOpen = false;
        craftingPanel.SetActive(false);
        craftingBG.SetActive(false);
        playerMovement.SetMovement(true);
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
            Item current = slot.GetItem();
            if (current is AbilityShardItem)
            {
                slot.slotIndex = i;
                slot.OnClick += MoveToInventory;
            }
        }
    }

    private void MoveToCraft(int itemIndex)
    {
        AbilityShardItem shard = itemSlots[itemIndex].GetItem() as AbilityShardItem;
        if (shard == null)
        {
            return;
        }

        int emptyIndex = -1;
        for (int i = 0; i < craftSlots.Length; i++)
        {
            if (craftSlots[i].GetItem() == null)
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
        AbilityShardItem shard = craftSlots[craftIndex].GetItem() as AbilityShardItem;
        if (shard == null)
        {
            return;
        }

        craftSlots[craftIndex].Clear();
        itemsInventory.AddItem(shard);
        RefreshUI();
    }
}
