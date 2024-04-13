using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected Player player;
    protected bool playerIsInTrigger = false;

    private void Start()
    {
        player = Player.instance;
        player.GetInputManager.OnInteract += HandleInteract;
    }

    protected virtual void HandleInteract() {
        playerIsInTrigger = false;
        player.ShowInteractUI(false);
    }
}
