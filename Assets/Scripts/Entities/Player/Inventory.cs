using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Gold playerGold = new Gold();

    public void SetUpData(PlayerData playerData)
    {
        playerGold.AddMultiplier(Attribute.Strength, playerData.heroStrength);
        playerGold.AddMultiplier(Attribute.Coordination, playerData.heroCoordination);
        playerGold.AddMultiplier(Attribute.Intelligence, playerData.heroIntelligence);
        playerGold.AddMultiplier(Attribute.Neutrality, playerData.heroNeutrality);
    }

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
