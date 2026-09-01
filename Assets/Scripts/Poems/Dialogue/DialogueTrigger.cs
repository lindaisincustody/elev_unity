using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : Interactable
{
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private bool instant = false;
    [SerializeField] private Material newMaterial;
    [SerializeField] private SpriteRenderer targetRenderer;
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
            ActivateDialogueInstantly();
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

        Trigger();
    }

    protected virtual void Trigger()
    {
        ActivateDialogue();
    }

    protected void ActivateDialogueInstantly()
    {
        ActivateDialogue();
        UIManager.Instance.Get<DialogueController>().NextAction();
    }

    private void HandleItemAddition()
    {
        if (itemAdded || itemToAdd == null) return;

        Player.instance.Get<ItemsInventory>().AddItem(itemToAdd);
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

    public void ChangeMaterial()
    {
        if (targetRenderer != null && newMaterial != null)
        {
            targetRenderer.material = newMaterial;
        }
    }

    public void ActivateDialogue()
    {
        UIManager.Instance.Get<DialogueController>().ActivateDialogue(dialogueData, this);
    }
}