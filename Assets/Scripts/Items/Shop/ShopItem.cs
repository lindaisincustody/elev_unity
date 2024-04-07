using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newShopItem", menuName = "Custom/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite sprite;
    public MiniGameItem item;
    public int cost;
}
