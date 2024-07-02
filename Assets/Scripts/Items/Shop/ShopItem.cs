using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "newShopItem", menuName = "Custom/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemId;
    public string itemName;
    [TextArea]
    public string description;
    public Sprite sprite;
    public Item item;
    public int cost;
}
