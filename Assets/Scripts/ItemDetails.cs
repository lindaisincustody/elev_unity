using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetails : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public TextMeshProUGUI descriptionText; // Add a reference to the description TextMeshPro component

    public string itemName;
    public Sprite itemIcon;
    public string description; // Store the description

    public void UpdateUI()
    {
        nameText.text = itemName;
        iconImage.sprite = itemIcon;
    }

}