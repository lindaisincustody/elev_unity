using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Gold playerGold = new Gold();

    private void Start()
    {
        SetUpData();
    }

    private void SetUpData()
    {
        float heroStrength = PlayerPrefs.GetFloat(Constants.PlayerPrefs.StrengthMultiplier);
        float heroCoordination = PlayerPrefs.GetFloat(Constants.PlayerPrefs.CoordinationMultiplier);
        float heroIntelligence = PlayerPrefs.GetFloat(Constants.PlayerPrefs.IntelligenceMultiplier);
        float heroNeutrality = PlayerPrefs.GetFloat(Constants.PlayerPrefs.NeutralityMultiplier);

        playerGold.AddMultiplier(Attribute.Strength, heroStrength);
        playerGold.AddMultiplier(Attribute.Coordination, heroCoordination);
        playerGold.AddMultiplier(Attribute.Intelligence, heroIntelligence);
        playerGold.AddMultiplier(Attribute.Neutrality, heroNeutrality);
    }

    private void UpdateMultiplier(Attribute attribute)
    {
        switch (attribute)
        {
            case (Attribute.Strength):
                PlayerPrefs.SetFloat(Constants.PlayerPrefs.StrengthMultiplier, GetGoldMultiplier(attribute));
                break;
            case (Attribute.Intelligence):
                PlayerPrefs.SetFloat(Constants.PlayerPrefs.IntelligenceMultiplier, GetGoldMultiplier(attribute));
                break;
            case (Attribute.Coordination):
                PlayerPrefs.SetFloat(Constants.PlayerPrefs.CoordinationMultiplier, GetGoldMultiplier(attribute));
                break;
            case (Attribute.Neutrality):
                PlayerPrefs.SetFloat(Constants.PlayerPrefs.NeutralityMultiplier, GetGoldMultiplier(attribute));
                break;
        }
    }

    public void AddGoldMultiplier(Attribute attribute, float multiplier)
    {
        playerGold.AddMultiplier(attribute, multiplier);
        UpdateMultiplier(attribute);
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
