using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] Inventory inventory;

    [SerializeField] TextMeshProUGUI gold;
    
    void Start()
    {
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        gold.text = inventory.GetGold().ToString();
    }
}
