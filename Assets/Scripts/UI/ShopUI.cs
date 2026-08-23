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
        playerInput = InputManager.Instance;

        playerInput.OnUICancel += ExitShop;
    }

    private void ExitShop()
    {
        if (!panel.activeSelf)
            return;
        panel.SetActive(false);
        player.SetMovement(true);
        itemsSelection.SetShopOpenState(false);
        UIManager.Instance.NotifyClosed(this);
    }

    public void ShowShop()
    {
        if (!UIManager.Instance.RequestOpen(this))
            return;

        UpdateGoldText();
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
