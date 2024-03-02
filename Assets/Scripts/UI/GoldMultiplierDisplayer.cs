using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldMultiplierDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI intelligenceText;
    [SerializeField] private TextMeshProUGUI coordinationText;
    [SerializeField] private TextMeshProUGUI neutralText;
    [SerializeField] private Inventory inventory;

    private readonly string strengthPrefix = "StrengthGold: ";
    private readonly string intelligencePrefix = "IntelligenceGold: ";
    private readonly string coordinationPrefix = "CoordinationGold: ";
    private readonly string neutralPrefix = "NeutralityGold: ";

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        strengthText.text = strengthPrefix + inventory.GetGoldMultiplier(Attribute.Strength).ToString("F2");
        intelligenceText.text = intelligencePrefix + inventory.GetGoldMultiplier(Attribute.Inteliigence).ToString("F2");
        coordinationText.text = coordinationPrefix + inventory.GetGoldMultiplier(Attribute.Coordination).ToString("F2");
        neutralText.text = neutralPrefix + inventory.GetGoldMultiplier(Attribute.Neutrality).ToString("F2");
    }
}
