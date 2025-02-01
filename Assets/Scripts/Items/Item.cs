using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "newItem", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public string itemId;
    public string itemName;
    [TextArea]
    public string description;
    public Sprite sprite;
    public Ability ability;
}
