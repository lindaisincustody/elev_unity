using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem strengthPS;
    [SerializeField] private ParticleSystem intelligencePS;
    [SerializeField] private ParticleSystem coordinationPS;
    [SerializeField] private ParticleSystem neutralityPS;

    private DialogueData dialogueData;

    public void SetDialogueData(DialogueData newData)
    {
        dialogueData = newData;
    }

    public void ActivatePSS(Attribute attribute)
    {
        DeasctivateAllPS();
        switch (attribute)
        {
            case Attribute.Strength:
                var strengthShape = strengthPS.shape;
                strengthShape.arc = (dialogueData.strengthGameCoinsMultiplier - 1) * 80;
                if (strengthShape.arc > 1)
                    strengthPS.gameObject.SetActive(true);
                break;
            case Attribute.Intelligence:
                var intelligenceShape = intelligencePS.shape;
                intelligenceShape.arc = (dialogueData.intelligenceGameCoinsMultiplier - 1) *80;
                if (intelligenceShape.arc > 1)
                    intelligencePS.gameObject.SetActive(true);
                break;
            case Attribute.Coordination:
                var coordinationShape = coordinationPS.shape;
                coordinationShape.arc = (dialogueData.coordinationGameCoinsMultiplier - 1) *80;
                if (coordinationShape.arc > 1)
                    coordinationPS.gameObject.SetActive(true);
                break;
            case Attribute.Neutrality:
                var neutralityShape = neutralityPS.shape;
                neutralityShape.arc = (dialogueData.neutralityGameCoinsMultiplier - 1) *80;
                if (neutralityShape.arc > 1)
                    neutralityPS.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown attribute!");
                break;
        }
    }

    private void DeasctivateAllPS()
    {
        strengthPS.gameObject.SetActive(false);
        intelligencePS.gameObject.SetActive(false);
        coordinationPS.gameObject.SetActive(false);
        neutralityPS.gameObject.SetActive(false);
    }
}
