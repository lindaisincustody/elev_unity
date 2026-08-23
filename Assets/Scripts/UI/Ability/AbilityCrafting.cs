using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCrafting : MonoBehaviour
{

    [SerializeField] GameObject craftingPanel;
    [SerializeField] GameObject craftingBG;
    [SerializeField] InventoryItemSlot[] itemSlots;
    [SerializeField] InventoryItemSlot[] craftSlots;
    [SerializeField] GameObject[] warningSlots;
    [SerializeField] GameObject explanation;
    [SerializeField] Image craftButton;

    Player player;
    ItemsInventory itemsInventory;
    PlayerMovement playerMovement;
    InputManager playerInput;

    bool isCraftingOpen;

    void Start()
    {
        player = Player.instance;
        playerMovement = player.Get<PlayerMovement>();
        itemsInventory = player.Get<ItemsInventory>();
        playerInput = InputManager.Instance;
        playerInput.OnUICancel += ClosePanel;
    }

    void OnDestroy()
    {
        if (isCraftingOpen) ReturnCraftItemsToInventory();
        playerInput.OnUICancel -= ClosePanel;
    }

    public void ToggleCrafting()
    {
        if (isCraftingOpen) ClosePanel();
        else OpenPanel();
    }

    public void OpenPanel()
    {
        if (isCraftingOpen) return;

        if (!UIManager.Instance.RequestOpen(this)) return;

        RefreshUI();
        UpdateCraftButton();
        isCraftingOpen = true;
        craftingPanel.SetActive(true);
        craftingBG.SetActive(true);
        playerMovement.SetMovement(false);
    }

    public void ClosePanel()
    {
        if (!isCraftingOpen) return;

        ReturnCraftItemsToInventory();
        isCraftingOpen = false;
        craftingPanel.SetActive(false);
        craftingBG.SetActive(false);
        playerMovement.SetMovement(true);
        UIManager.Instance.NotifyClosed(this);
    }

    public void Craft()
    {
        List<AbilityTier> tiers = new();
        foreach (var slot in craftSlots)
        {
            if (slot.Item == null) return;
            tiers.Add(((AbilityShardItem)slot.Item).Tier);
        }

        UIManager.Instance.Get<AbilitySelectionUI>().Show(tiers);

        foreach (var slot in craftSlots)
            if (slot.Item != null) slot.Clear();

        ClosePanel();
    }

    void RefreshUI()
    {
        UpdateItemSlots();
        UpdateCraftSlots();
    }

    void UpdateCraftButton()
    {
        craftButton.color = craftSlots.All(slot => slot.Item != null) ? Color.green : Color.white;
    }

    void UpdateItemSlots()
    {
        var shards = itemsInventory.GetAllItems().Where(i => i is AbilityShardItem).ToList();
        var grouped = shards.GroupBy(i => i.itemId).ToList();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Clear();
            itemSlots[i].OnClick = null;
        }

        for (int i = 0; i < grouped.Count && i < itemSlots.Length; i++)
        {
            var shard = (AbilityShardItem)grouped[i].First();
            itemSlots[i].Equip(shard, grouped[i].Count());
            itemSlots[i].slotIndex = i;
            itemSlots[i].OnClick += MoveToCraft;
        }
    }

    void UpdateCraftSlots()
    {
        for (int i = 0; i < craftSlots.Length; i++)
        {
            var slot = craftSlots[i];
            slot.OnClick = null;
            if (slot.Item is AbilityShardItem)
            {
                slot.slotIndex = i;
                slot.OnClick += MoveToInventory;
            }
        }
        UpdateWarningSlots();
    }

    void MoveToCraft(int itemIndex)
    {
        var shard = itemSlots[itemIndex].Item as AbilityShardItem;
        if (shard == null) return;

        int emptyIndex = System.Array.FindIndex(craftSlots, s => s.Item == null);
        if (emptyIndex < 0)
        {
            UpdateCraftButton();
            UpdateWarningSlots();
            return;
        }

        var craftSlot = craftSlots[emptyIndex];
        craftSlot.Equip(shard, 1);
        craftSlot.slotIndex = emptyIndex;
        craftSlot.OnClick += MoveToInventory;

        itemsInventory.RemoveItem(shard);

        int remaining = itemsInventory.GetAllItems().Count(i => i.itemId == shard.itemId);
        if (remaining > 0) itemSlots[itemIndex].Equip(shard, remaining);
        else RefreshUI();

        UpdateCraftButton();
        UpdateWarningSlots();
    }

    void MoveToInventory(int craftIndex)
    {
        var shard = craftSlots[craftIndex].Item as AbilityShardItem;
        if (shard == null) return;

        craftSlots[craftIndex].Clear();
        itemsInventory.AddItem(shard);

        RefreshUI();
        UpdateCraftButton();
        UpdateWarningSlots();
    }

    void ReturnCraftItemsToInventory()
    {
        foreach (var slot in craftSlots)
        {
            var shard = slot.Item as AbilityShardItem;
            if (shard != null)
            {
                itemsInventory.AddItem(shard);
                slot.Clear();
            }
        }
    }

    void UpdateWarningSlots()
    {
        explanation.SetActive(false);
        foreach (var w in warningSlots) w.SetActive(false);

        var playerAbilities = Player.instance.GetComponent<PlayerAbilities>().Abilities;
        var remaining = UIManager.Instance.Get<AbilitySelectionUI>().availableAbilities
            .Where(a => !playerAbilities.Contains(a))
            .ToList();

        List<Ability> used = new();

        for (int i = 0; i < craftSlots.Length; i++)
        {
            var shard = craftSlots[i].Item as AbilityShardItem;
            if (shard == null) continue;

            Ability match = null;
            bool fallback = false;

            for (int t = (int)shard.Tier; t >= (int)AbilityTier.Tier1; t--)
            {
                match = remaining.FirstOrDefault(a => (int)a.Tier == t && !used.Contains(a));
                if (match != null)
                {
                    used.Add(match);
                    fallback = (AbilityTier)t != shard.Tier;
                    break;
                }
            }

            if ((match == null || fallback) && i < warningSlots.Length)
            {
                warningSlots[i].SetActive(true);
                explanation.SetActive(true);
            }
        }
    }
}
