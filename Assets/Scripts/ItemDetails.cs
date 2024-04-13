using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetails : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI nameText;
    public Image iconImage;

    public void UpdateUI(string gold, string name, Sprite icon)
    {
        nameText.text = name;
        goldText.text = gold;
        iconImage.sprite = icon;
    }
}