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
    PlayerMovement playerMovement;
    PlayerWallet wallet;

    void Start()
    {
        playerMovement = Player.instance.Get<PlayerMovement>();
        wallet = Player.instance.Get<PlayerWallet>();
        playerInput = InputManager.Instance;

        playerInput.OnUICancel += ExitShop;
    }

    private void ExitShop()
    {
        if (!panel.activeSelf)
            return;
        panel.SetActive(false);
        playerMovement.UnlockMovement(this);
        itemsSelection.SetShopOpenState(false);
        UIManager.Instance.NotifyClosed(this);
    }

    public void ShowShop()
    {
        if (!UIManager.Instance.RequestOpen(this))
            return;

        UpdateGoldText();
        panel.SetActive(true);
        playerMovement.LockMovement(this);
        itemsSelection.SetShopOpenState(true);
    }

    public void RefreshShopUI()
    {
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        gold.text = wallet.GetGold().ToString();
    }

    private void OnDestroy()
    {
        playerInput.OnUICancel -= ExitShop;
    }
}
