using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private void Awake()
    {
        playerInput.OnInteract += NextAction;
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
            DeactivateDialogue();
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

    private void DeactivateDialogue()
    {
        isDialogueActive = false;
        DialogueObj.SetActive(false);
        playerMovement.SetMovement(true);
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

    private void ShowMinigameOptions()
    {
        isDialogueActive = false;
        MinigamesBoxObj.SetActive(true);
        cursor.ActivateCursor(minigamebox.cursorElements);
    }

    private void OnDisable()
    {
        playerInput.OnInteract -= NextAction;
    }
}
