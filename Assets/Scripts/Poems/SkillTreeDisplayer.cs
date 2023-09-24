using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private Image strengthFillImage;
    [SerializeField] private Image intelligenceFillImage;
    [SerializeField] private Image coordinationFillImage;
    [SerializeField] private Image neutralityFillImage;

    [SerializeField] private Attributes heroAtrributes;

    public void UpdateSkilltree(int numberOfPoems)
    {
        strengthFillImage.fillAmount = heroAtrributes.heroStrength / numberOfPoems;
        intelligenceFillImage.fillAmount = heroAtrributes.heroIntelligence / numberOfPoems;
        coordinationFillImage.fillAmount = heroAtrributes.heroCoordination / numberOfPoems;
        neutralityFillImage.fillAmount = heroAtrributes.heroNeutrality / numberOfPoems;
    }
}
