using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionButton : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI AbilityNameText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI DescriptionText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TierText { get; private set; }
    [field: SerializeField] public Image AbilityIcon{ get; private set; }
}
