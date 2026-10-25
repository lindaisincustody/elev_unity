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

    PlayerWallet wallet;

    // Start is called before the first frame update
    void Start()
    {
        wallet = Player.instance.Get<PlayerWallet>();
        UpdateText();
    }

    public void UpdateText()
    {
        strengthText.text = strengthPrefix + wallet.GetMultiplier(Attribute.Strength).ToString("F2");
        intelligenceText.text = intelligencePrefix + wallet.GetMultiplier(Attribute.Intelligence).ToString("F2");
        coordinationText.text = coordinationPrefix + wallet.GetMultiplier(Attribute.Coordination).ToString("F2");
        neutralText.text = neutralPrefix + wallet.GetMultiplier(Attribute.Neutrality).ToString("F2");
    }
}
