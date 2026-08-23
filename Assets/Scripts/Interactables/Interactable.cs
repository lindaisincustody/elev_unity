using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected Player player;
    protected bool playerIsInTrigger = false;

    protected virtual void Start()
    {
        player = Player.instance;
        InputManager.Instance.OnInteract += HandleInteract;
    }

    protected virtual void HandleInteract() {
        playerIsInTrigger = false;
        player.ShowInteractUI(false);
    }

    protected void OnDestroy()
    {
        player = Player.instance;
        InputManager.Instance.OnInteract -= HandleInteract;
    }
}
