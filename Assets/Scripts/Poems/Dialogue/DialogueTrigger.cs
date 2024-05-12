using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueTrigger : Interactable
{
    [SerializeField] public PlayableDirector director;
    [SerializeField] DialogueController dialogueController;
    [SerializeField] DialogueData dialogueData;
    [SerializeField] private bool isPopup = false; // Determines trigger type
    [SerializeField] private Material newMaterial; // New material to apply after dialogue
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private bool isDoorInteraction;
    [SerializeField] private bool isInteractionCircle;
    [SerializeField] private ShopItem itemToAdd;
    private bool itemAdded = false;

    public int expAmount = 100;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (!itemAdded && itemToAdd != null)
            {
                ItemsInventory.Instance.AddItem(itemToAdd);
                itemAdded = true; // Set the flag to true so the item isn't added again
            }
            playerIsInTrigger = true;
            if (isPopup)
            {
                


                // Automatically activate the dialogue if it's a popup
                dialogueController.ActivateDialogue(dialogueData, this);
                dialogueController.NextAction();


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
            ExperienceBar.instance.AddExperience(expAmount);

            
            if (isInteractionCircle)
            {
                Transform child = transform.Find("InteractionCircle");
                if (child != null)
                {
                    SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = false; // Disable the SpriteRenderer
                    }
                }
            }
            if (isDoorInteraction)
            {
                // Subscribe to the stopped event
                director.stopped += OnPlaybackStopped;
                director.Play();
            }
            else if (!isDoorInteraction)
            {
                dialogueController.ActivateDialogue(dialogueData, this);
            }
        }
    }

    private void OnPlaybackStopped(PlayableDirector aDirector)
    {
        // Unsubscribe to prevent the event from being called multiple times
        director.stopped -= OnPlaybackStopped;

        // Check if the stopped director is the one we're interested in
        if (aDirector == director)
        {
            dialogueController.ActivateDialogue(dialogueData, this);
            dialogueController.NextAction();
        }
    }
    public void ChangeMaterial()
    {
        if (targetRenderer != null && newMaterial != null)
        {
            targetRenderer.material = newMaterial;
        }
    }
    public void ActivateDialogue()
    {
        if (dialogueController != null)
        {
            dialogueController.ActivateDialogue(dialogueData, this); // Pass this reference
        }
    }
}