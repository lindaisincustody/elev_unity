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

    private readonly string strengthPrefix = "StrengthGold: ";
    private readonly string intelligencePrefix = "IntelligenceGold: ";
    private readonly string coordinationPrefix = "CoordinationGold: ";
    private readonly string neutralPrefix = "NeutralityGold: ";

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        UpdateText();
    }

    public void UpdateText()
    {
        strengthText.text = strengthPrefix + player.GetGoldMultiplier(Attribute.Strength).ToString("F2");
        intelligenceText.text = intelligencePrefix + player.GetGoldMultiplier(Attribute.Intelligence).ToString("F2");
        coordinationText.text = coordinationPrefix + player.GetGoldMultiplier(Attribute.Coordination).ToString("F2");
        neutralText.text = neutralPrefix + player.GetGoldMultiplier(Attribute.Neutrality).ToString("F2");
    }
}
