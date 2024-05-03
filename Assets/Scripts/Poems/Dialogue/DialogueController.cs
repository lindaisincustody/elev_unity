using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueController : MonoBehaviour
{
    [Header("DialogueBox References")]
    [SerializeField] GameObject CanvasBG;
    [SerializeField] GameObject DialogueObj;
    [SerializeField] GameObject NarratorObj;
    [SerializeField] GameObject MinigamesBoxObj;
    [SerializeField] TextMeshProUGUI dialogueText;    
    [SerializeField] TextMeshProUGUI narratorText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image mainCharacterImage;
    [SerializeField] Image otherCharacterImage;
    [SerializeField] GameObject mainCharacterImageHolder;
    [SerializeField] GameObject otherCharacterImageHolder;
    [SerializeField] AttributeParticles particles;
    [Header("Cursor References")]
    [SerializeField] CursorController cursor;
    [SerializeField] UIElementsHolder minigamebox;
    [Header("Attribute Bar References")]
    [SerializeField] GameObject multiplierFrame;
    [SerializeField] Image strengthRect;
    [SerializeField] Image intelligenceRect;
    [SerializeField] Image coordinationRect;
    [SerializeField] Image neutralityRect;

    MinigameUI minigameUI;
    DialogueUI dialogueUI;

    Player player;
    InputManager playerInput;
    PlayerMovement playerMovement;

    DialogueData dialogueData;
    Coroutine dialogueCoroutine;
    Coroutine cursorCoroutine;
    private int currentDialogueLine = 0;
    private float delay = 0.05f;
    private string fullText;
    private string currentText;

    private bool isDialogueActive = false;
    private bool isMinigamesBoxActive = false;
    private DialogueTrigger currentTrigger;

    private void Start()
    {
        player = Player.instance;
        playerInput = player.GetInputManager;
        playerMovement = player.GetPlayerMovement;
        playerInput.OnInteract += NextAction;
        playerInput.OnUICancel += ExitDialogue;

        minigameUI = new MinigameUI(MinigamesBoxObj, multiplierFrame, strengthRect, intelligenceRect, coordinationRect, neutralityRect);
        dialogueUI = new DialogueUI(DialogueObj, NarratorObj, dialogueText, nameText, mainCharacterImage, otherCharacterImage, mainCharacterImageHolder, otherCharacterImageHolder, minigameUI);
    }

    private void OnDestroy()
    {
        playerInput.OnInteract -= NextAction;
        playerInput.OnUICancel -= ExitDialogue;
    }

    public void ActivateDialogue(DialogueData newDialogueData, DialogueTrigger trigger)
    {
        currentTrigger = trigger;
        dialogueData = newDialogueData;
        InventoryUI.Instance.CanOpenInventory(false);
        currentDialogueLine = 0;
        isDialogueActive = true;
        if (newDialogueData.dialogueType == DialogueType.Dialogue)
        {
            ActivateDialogue(newDialogueData);
        }
        else if (newDialogueData.dialogueType == DialogueType.SelfDialogue)
        {
            ActivateSelfDialogue(newDialogueData);
        }
        else if (newDialogueData.dialogueType == DialogueType.Narrator)
        {
            ActivateNarrator(newDialogueData);
        }
        //NextAction();
    }

    private void ActivateDialogue(DialogueData newDialogueData)
    {
        CanvasBG.SetActive(true);
        particles.SetDialogueData(newDialogueData);
        playerMovement.SetMovement(false);
        dialogueUI.ActivateDialogueBox(newDialogueData);
    }

    private void ActivateSelfDialogue(DialogueData newDialogueData)
    {
        CanvasBG.SetActive(true);
        playerMovement.SetMovement(false);
        dialogueUI.ActivateDialogueBox(newDialogueData);
    }

    private void ActivateNarrator(DialogueData newDialogueData)
    {
        playerMovement.SetMovement(false);
        dialogueUI.ActivateNarratorBox(newDialogueData);
    }

    public void NextAction()
    {
        if (!isDialogueActive)
            return;

        if (dialogueData.dialogueType != DialogueType.Narrator)
        {
            // Check if the text is fully displayed or being displayed
            if (dialogueCoroutine != null && dialogueText.text != fullText)
            {
                StopCoroutine(dialogueCoroutine); // Stop the currently running coroutine
                dialogueText.text = fullText; // Immediately display the full text
                dialogueCoroutine = null; // Reset coroutine variable
            }
            else
            {
                // Increment dialogue line or end dialogue
                if (currentDialogueLine < dialogueData.textList.Length)
                    ShowNextDialogueLine();
                else if (dialogueData.activateFight)
                    ShowMinigameOptions();
                else
                    ExitDialogue();
            }
        }
        else
        {
            if (dialogueCoroutine != null && narratorText.text != fullText)
            {
                StopCoroutine(dialogueCoroutine); // Stop the currently running coroutine
                narratorText.text = fullText; // Immediately display the full text
                dialogueCoroutine = null; // Reset coroutine variable
            }
            else
            {
                // Increment dialogue line or end dialogue
                if (currentDialogueLine < dialogueData.textList.Length)
                {
                    ShowNextDialogueLine();
                }
                else
                    ExitDialogue();
            }
        }
    }

    private void ShowNextDialogueLine()
    {
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueUI.ShowNext(currentDialogueLine);
        if (dialogueData.textList[currentDialogueLine].lineType != LineType.Narrator)
            dialogueCoroutine = StartCoroutine(ShowText());
        else
            dialogueCoroutine = StartCoroutine(ShowNarratorText());
        currentDialogueLine++; // Move to the next line after setting up the coroutine
    }

    IEnumerator ShowText()
    {
        fullText = dialogueData.textList[currentDialogueLine].dialogueLineText;
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            dialogueText.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator ShowNarratorText()
    {
        fullText = dialogueData.textList[currentDialogueLine].dialogueLineText;
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            narratorText.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }

    public void ExitDialogue()
    {
        if (!isDialogueActive && !isMinigamesBoxActive) return;

        InventoryUI.Instance.CanOpenInventory(true);
        isDialogueActive = false;
        dialogueData = null;
        currentDialogueLine = 0;

        dialogueUI.Hide();
        minigameUI.Hide();

        playerMovement.SetMovement(true);
        cursor.DeactivateCursor();
        if (cursorCoroutine != null)
            StopCoroutine(cursorCoroutine);
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine = null;
        cursorCoroutine = null;
        CanvasBG.SetActive(false);

        if (currentTrigger != null)
        {
            currentTrigger.ChangeMaterial();
            
            currentTrigger = null; // Reset the trigger reference after use
        }
    }

    private void ShowMinigameOptions()
    {
        dialogueUI.ShowNext(currentDialogueLine);
        minigameUI.Show(dialogueData);
        isDialogueActive = false;
        isMinigamesBoxActive = true;
        cursorCoroutine = StartCoroutine(ActivateCursor());
    }

    private IEnumerator ActivateCursor()
    {
        yield return new WaitForSeconds(0.3f);
        cursor.ActivateCursor(minigamebox.cursorElements, () => isMinigamesBoxActive = false);
    }

    public void ChosenGame(int gameAttribute)
    {
        switch (gameAttribute)
        {
            case 0:
                ActionManager.OnGoldMultiplierChange.Invoke(Attribute.Strength, dialogueData.strengthGameCoinsMultiplier);
                break;
            case 1:
                ActionManager.OnGoldMultiplierChange.Invoke(Attribute.Coordination, dialogueData.coordinationGameCoinsMultiplier);
                break;
            case 2:
                ActionManager.OnGoldMultiplierChange.Invoke(Attribute.Intelligence, dialogueData.intelligenceGameCoinsMultiplier);
                break;
            case 3:
                ActionManager.OnGoldMultiplierChange.Invoke(Attribute.Neutrality, dialogueData.neutralityGameCoinsMultiplier);
                break;
        }

    }
}
