using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAdjuster : MonoBehaviour
{
    [SerializeField] RectTransform strength;
    [SerializeField] RectTransform intelligence;
    [SerializeField] RectTransform coordination;
    [SerializeField] RectTransform neutrality;

    private Vector3 increasedSize = new Vector3(1.1f, 1.1f, 1);

    public void markAttribute(Attribute attribute)
    {
        ResetScales();
        switch (attribute)
        {
            case Attribute.Strength:
                strength.localScale = increasedSize;
                break;
            case Attribute.Intelligence:
                intelligence.localScale = increasedSize;
                break;
            case Attribute.Coordination:
                coordination.localScale = increasedSize;
                break;
            case Attribute.Neutrality:
                neutrality.localScale = increasedSize;
                break;
            default:
                Debug.LogWarning("Unknown attribute!");
                break;
        }
    }

    public void ResetScales()
    {
        strength.localScale = Vector3.one;
        intelligence.localScale = Vector3.one;
        coordination.localScale = Vector3.one;
        neutrality.localScale = Vector3.one;
    }
}
