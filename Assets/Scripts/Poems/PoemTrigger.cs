using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoemTrigger : Interactable
{
    [SerializeField] private WordData wordsData;

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
            PoemMenuController.instance.OpenPoemBook(wordsData);
            gameObject.SetActive(false);
        }
    }
}
