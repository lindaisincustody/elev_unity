using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] GameObject panel;
    [SerializeField] PanelItemsSection itemsSelection;

    InputManager playerInput;
    Player player;

    void Start()
    {
        player = Player.instance;
        playerInput = player.GetInputManager;

        playerInput.OnUICancel += ExitShop;
    }

    private void ExitShop()
    {
        if (!panel.activeSelf)
            return;
        InventoryUI.Instance.CanOpenInventory(true);
        panel.SetActive(false);
        player.SetMovement(true);
        itemsSelection.SetShopOpenState(false);
    }

    public void ShowShop()
    {
        UpdateGoldText();
        InventoryUI.Instance.CanOpenInventory(false);
        panel.SetActive(true);
        player.SetMovement(false);
        itemsSelection.SetShopOpenState(true);
    }

    public void RefreshShopUI()
    {
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        gold.text = player.GetGold().ToString();
    }

    private void OnDestroy()
    {
        playerInput.OnUICancel -= ExitShop;
    }
}
