using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueController : MonoBehaviour
{
    [Header("DialogueBox References")]
    [SerializeField] public GameObject CanvasBG;
    [SerializeField] public GameObject DialogueObj;
    [SerializeField] public GameObject MinigamesBoxObj;
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public Image mainCharacterImage;
    [SerializeField] public Image otherCharacterImage;
    [SerializeField] public GameObject mainCharacterImageHolder;
    [SerializeField] public GameObject otherCharacterImageHolder;
    [SerializeField] public AttributeParticles particles;
    [Header("Cursor References")]
    [SerializeField] public CursorController cursor;
    [SerializeField] public UIElementsHolder minigamebox;
    [Header("Attribute Bar References")]
    [SerializeField] public GameObject multiplierFrame;
    [SerializeField] public Image strengthRect;
    [SerializeField] public Image intelligenceRect;
    [SerializeField] public Image coordinationRect;
    [SerializeField] public Image neutralityRect;

    public MinigameUI minigameUI;
    public DialogueUI dialogueUI;

    public Player player;
    public  InputManager playerInput;
    public PlayerMovement playerMovement;

    public DialogueData dialogueData;
    public  Coroutine dialogueCoroutine;
    public int currentDialogueLine = 0;
    private float delay = 0.05f;
    private string fullText;
    private string currentText;

    public bool isDialogueActive = false;
    public bool isMinigamesBoxActive = false;
    private DialogueTrigger currentTrigger;

    private void Start()
    {
        player = Player.instance;
        playerInput = player.GetInputManager;
        playerMovement = player.GetPlayerMovement;
        playerInput.OnInteract += NextAction;
        playerInput.OnUICancel += ExitDialogue;

        minigameUI = new MinigameUI(MinigamesBoxObj, multiplierFrame, strengthRect, intelligenceRect, coordinationRect, neutralityRect);
        dialogueUI = new DialogueUI(DialogueObj, dialogueText, nameText, mainCharacterImage, otherCharacterImage, mainCharacterImageHolder, otherCharacterImageHolder, minigameUI);
    }

    private void OnDestroy()
    {
        playerInput.OnInteract -= NextAction;
        playerInput.OnUICancel -= ExitDialogue;
    }

    public void ActivateDialogue(DialogueData newDialogueData, DialogueTrigger trigger)
    {
        currentTrigger = trigger;
        InventoryUI.Instance.CanOpenInventory(false);
        CanvasBG.SetActive(true);
        particles.SetDialogueData(newDialogueData);
        dialogueData = newDialogueData;
        currentDialogueLine = 0;
        isDialogueActive = true;

        //playerMovement.SetMovement(false);
        dialogueUI.ActivateDialogueBox(newDialogueData);

        //NextAction();
    }

    public void NextAction()
    {
        if (!isDialogueActive)
            return;

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
    public void ShowNextDialogueLine()
    {
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueUI.ShowNext(currentDialogueLine);
        dialogueCoroutine = StartCoroutine(ShowText());
        currentDialogueLine++; // Move to the next line after setting up the coroutine
    }

    public IEnumerator ShowText()
    {
        fullText = dialogueData.textList[currentDialogueLine].dialogueLineText;
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            dialogueText.text = currentText;
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

        if (playerMovement != null)
        playerMovement.SetMovement(true);
        cursor.DeactivateCursor();
        CanvasBG.SetActive(false);

        if (currentTrigger != null)
        {
            currentTrigger.ChangeMaterial();
            currentTrigger = null; // Reset the trigger reference after use
        }
    }

    public void ShowMinigameOptions()
    {
        dialogueUI.ShowNext(currentDialogueLine);
        minigameUI.Show(dialogueData);
        isDialogueActive = false;
        isMinigamesBoxActive = true;
        StartCoroutine(ActivateCursor());
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
