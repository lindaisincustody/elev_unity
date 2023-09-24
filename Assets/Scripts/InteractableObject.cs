using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InteractableObject : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    private int index = 0;

    public float wordSpeed;
    public bool playerIsClose;

    private InputManager playerInput;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<InputManager>();
    }

    private void Start()
    {
        playerInput.OnInteract += ShowDialogue;
        playerInput.OnCancel += HideDialogue;

        dialogueText.text = "";
    }

    public void ShowDialogue()
    {
        if (!playerIsClose)
            return;

        if (!dialoguePanel.activeInHierarchy)
        {
            dialoguePanel.SetActive(true);
            StartTyping();
        }
        else if (dialogueText.text == dialogue[index])
        {
            NextLine();
        }
    }

    public void HideDialogue()
    {
        RemoveText();
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    private void StartTyping()
    {
        StopTyping();
        dialogueText.text = ""; // Clear previous text
        typingCoroutine = StartCoroutine(Typing());
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
    }

    public void NextLine()
    {
        if (index < dialogue.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            HideDialogue();
        }
    }

    public void RemoveText()
    {
        StopTyping();
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            RemoveText();
        }
    }
}
