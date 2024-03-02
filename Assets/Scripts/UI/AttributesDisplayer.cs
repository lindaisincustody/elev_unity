using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttributesDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI intelligenceText;
    [SerializeField] private TextMeshProUGUI coordinationText;
    [SerializeField] private TextMeshProUGUI neutralText;
    [SerializeField] private Attributes heroAtrributes;

    private readonly string strengthPrefix = "Strength: ";
    private readonly string intelligencePrefix = "Intelligence: ";
    private readonly string coordinationPrefix = "Coordination: ";
    private readonly string neutralPrefix = "Neutrality: ";

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        strengthText.text = strengthPrefix + heroAtrributes.heroStrength.ToString("F2");
        intelligenceText.text = intelligencePrefix + heroAtrributes.heroIntelligence.ToString("F2");
        coordinationText.text = coordinationPrefix + heroAtrributes.heroCoordination.ToString("F2");
        neutralText.text = neutralPrefix + heroAtrributes.heroNeutrality.ToString("F2");
    }
}
