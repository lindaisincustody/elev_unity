using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingRecipes : MonoBehaviour
{
    [SerializeField] private CraftingRecipe[] craftingRecipes;

    public Item TryToCraft(InventoryItemSlot[] craftSlots)
    {
        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            List<Item> stillNeeded = new List<Item>(recipe.requiredItems);

            foreach (InventoryItemSlot slot in craftSlots)
            {
                if (slot.Item != null && stillNeeded.Contains(slot.Item))
                    stillNeeded.Remove(slot.Item);
            }

            if (stillNeeded.Count == 0)
                return recipe.resultItem;
        }

        return null;
    }

    [Serializable]
    public struct CraftingRecipe
    {
        public Item[] requiredItems;
        public Item resultItem;
    }
}
