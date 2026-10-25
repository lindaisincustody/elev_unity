using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool playerIsInTrigger = false;

    protected InteractPrompt interactPrompt => Player.instance.Get<InteractPrompt>();

    protected virtual void Start()
    {
        InputManager.Instance.OnInteract += HandleInteract;
    }

    protected virtual void HandleInteract() {
        playerIsInTrigger = false;
        interactPrompt.Show(false);
    }

    protected void OnDestroy()
    {
        InputManager.Instance.OnInteract -= HandleInteract;
    }
}
