using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    [SerializeField] DialogueController dialogueController;
    [SerializeField] DialogueData dialogueData;
    [SerializeField] private bool isPopup = false; // Determines trigger type

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            if (isPopup)
            {
                // Automatically activate the dialogue if it's a popup
                dialogueController.ActivateDialogue(dialogueData);
            }
            else
            {
                // Show interact prompt if it requires player interaction
                player.ShowInteractUI(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            if (!isPopup)
            {
                // Only hide the interact UI if it's not a popup
                player.ShowInteractUI(false);
            }
        }
    }

    protected override void HandleInteract()
    {
        if (playerIsInTrigger && !isPopup)
        {
            base.HandleInteract();
            dialogueController.ActivateDialogue(dialogueData);
        }
    }
}