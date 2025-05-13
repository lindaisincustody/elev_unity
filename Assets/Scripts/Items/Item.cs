using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "newItem", menuName = "Custom/Items/Item")]
public class Item : ScriptableObject
{
    public string itemId;
    public string itemName;
    [TextArea]
    public string description;
    public Sprite sprite;
    public virtual void Use() { }
}
