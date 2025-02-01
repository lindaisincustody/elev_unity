using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class DialogueTrigger : Interactable
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool instant = false;
    [SerializeField] private Material newMaterial;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private bool isDoorInteraction;
    [SerializeField] private bool isInteractionCircle;
    [SerializeField] private Item itemToAdd;
    [SerializeField] public UnityEvent OnComplete;
    private bool itemAdded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        HandleItemAddition();

        playerIsInTrigger = true;

        if (instant)
        {
            dialogueController?.ActivateDialogue(dialogueData, this);
            dialogueController?.NextAction();
        }
        else
        {
            player.ShowInteractUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            if (!instant)
            {
                player.ShowInteractUI(false);
            }
        }
    }

    protected override void HandleInteract()
    {
        if (!playerIsInTrigger || instant) return;

        base.HandleInteract();

        if (isInteractionCircle)
        {
            DisableInteractionCircle();
        }

        if (isDoorInteraction)
        {
            director.stopped += OnPlaybackStopped;
            director.Play();
        }
        else
        {
            dialogueController?.ActivateDialogue(dialogueData, this);
        }
    }

    private void HandleItemAddition()
    {
        if (itemAdded || itemToAdd == null) return;

        Player.instance.ItemsInventory.AddItem(itemToAdd);
        itemAdded = true;
    }

    private void DisableInteractionCircle()
    {
        Transform child = transform.Find("InteractionCircle");
        if (child == null) return;

        SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnPlaybackStopped(PlayableDirector aDirector)
    {
        if (aDirector != director) return;

        director.stopped -= OnPlaybackStopped;

        dialogueController?.ActivateDialogue(dialogueData, this);
        dialogueController?.NextAction();
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
        dialogueController?.ActivateDialogue(dialogueData, this);
    }
}