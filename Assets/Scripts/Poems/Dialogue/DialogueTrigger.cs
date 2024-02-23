using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] DialogueController dialogueController;
    [SerializeField] DialogueData dialogueData;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueController.ActivateDialogue(dialogueData);
        }
    }
}
