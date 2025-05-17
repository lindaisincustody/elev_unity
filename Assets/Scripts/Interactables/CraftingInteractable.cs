using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingInteractable : Interactable
{
    private CraftingUI craftingUI;

    protected override void Start()
    {
        base.Start();
        craftingUI = CraftingUI.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            player.ShowInteractUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            player.ShowInteractUI(false);
        }
    }

    protected override void HandleInteract()
    {
        if (playerIsInTrigger)
        {
            base.HandleInteract();
            craftingUI.OpenPanel();
        }
    }
}
