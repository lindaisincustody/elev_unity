using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Gold playerGold = new Gold();

    public void AddGoldMultiplier(Attribute attribute, float multiplier)
    {
        playerGold.AddMultiplier(attribute, multiplier);
    }

    public void AddGold(int ammount)
    {
        playerGold.AddGold(ammount);
    }

    public float GetGoldMultiplier(Attribute attribute)
    {
        return playerGold.GetMultiplier(attribute);
    }

    public int GetGold()
    {
        return playerGold.GetGold();
    }
}
