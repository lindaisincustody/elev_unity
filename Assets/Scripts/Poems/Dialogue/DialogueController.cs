using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueController : MonoBehaviour
{
    [Header("DialogueBox References")]
    [SerializeField] GameObject DialogueObj;
    [SerializeField] GameObject MinigamesBoxObj;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image mainCharacterImage;
    [SerializeField] Image otherCharacterImage;
    [Header("Player References")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] InputManager playerInput;
    [Header("Cursor References")]
    [SerializeField] CursorController cursor;
    [SerializeField] UIElementsHolder minigamebox;

    DialogueData dialogueData;
    Coroutine dialogueCoroutine;
    private int currentDialogueLine = 0;
    private float delay = 0.1f;
    private string fullText;
    private string currentText;

    private bool isDialogueActive = false;
    private bool isMinigamesBoxActive = false;

    private void Awake()
    {
        playerInput.OnInteract += NextAction;
        playerInput.OnUICancel += ExitDialogue;
    }

    private void OnDestroy()
    {
        playerInput.OnInteract -= NextAction;
        playerInput.OnUICancel -= ExitDialogue;
    }

    private void NextAction()
    {
        if (!isDialogueActive)
            return;

        if (dialogueData.textList.Length > currentDialogueLine)
            ShowNextDialogueLine();
        else if (dialogueData.activateFight)
            ShowMinigameOptions();
        else
            ExitDialogue();
    }

    private void ExitDialogue()
    {
        if (isDialogueActive || isMinigamesBoxActive)
        {
            isDialogueActive = false;
            isMinigamesBoxActive = false;
            DialogueObj.SetActive(false);
            MinigamesBoxObj.SetActive(false);
            playerMovement.SetMovement(true);
            dialogueData = null;
            currentDialogueLine = 0;
            cursor.DeactivateCursor();
        }
    }

    public void ActivateDialogue(DialogueData newDialogueData)
    {
        isDialogueActive = true;
        DialogueObj.SetActive(true);
        playerMovement.SetMovement(false);
        dialogueData = newDialogueData;
        currentDialogueLine = 0;
        mainCharacterImage.sprite = dialogueData.mainCharacterImage;
        otherCharacterImage.sprite = dialogueData.otherCharacterImage;
        dialogueCoroutine = StartCoroutine(ShowText());
    }

    private void ShowNextDialogueLine()
    {
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);
        dialogueText.text = string.Empty;
        if (dialogueData.textList[currentDialogueLine].isYourText)
            nameText.text = "You";
        else
            nameText.text = dialogueData.otherCharacterName;
        dialogueCoroutine = StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        fullText = dialogueData.textList[currentDialogueLine].dialogueLineText;
        currentDialogueLine++;
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            dialogueText.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }

    private void SetMiniboxActive(bool isActive)
    {
        isMinigamesBoxActive = isActive;
    }

    private void ShowMinigameOptions()
    {
        isDialogueActive = false;
        isMinigamesBoxActive = true;
        MinigamesBoxObj.SetActive(true);
        StartCoroutine(ActivateCursor());
    }

    private IEnumerator ActivateCursor()
    {
        yield return new WaitForSeconds(0.3f);
        cursor.ActivateCursor(minigamebox.cursorElements, () => SetMiniboxActive(false));
    }
}
