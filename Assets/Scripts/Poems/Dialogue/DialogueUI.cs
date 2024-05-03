using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI
{
    private GameObject dialogueBox;
    private GameObject narratorBox;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI nameText;
    private Image mainCharacterImage;
    private Image otherCharacterImage;
    private GameObject mainCharacterImageHolder;
    private GameObject otherCharacterImageHolder;

    private DialogueData dialogueData;
    private MinigameUI minigameUI;

    public DialogueUI(GameObject box, GameObject narratorbox, TextMeshProUGUI text, TextMeshProUGUI name, Image mainImage, Image otherImage, GameObject mainHolder, GameObject otherHolder, MinigameUI minigame)
    {
        dialogueBox = box;
        narratorBox = narratorbox;
        dialogueText = text;
        nameText = name;
        mainCharacterImage = mainImage;
        otherCharacterImage = otherImage;
        mainCharacterImageHolder = mainHolder;
        otherCharacterImageHolder = otherHolder;
        minigameUI = minigame;
    }

    public void ActivateDialogueBox(DialogueData newDialogueData)
    {
        dialogueData = newDialogueData;
        dialogueBox.SetActive(true);
        mainCharacterImage.sprite = dialogueData.mainCharacterImage;
        otherCharacterImage.sprite = dialogueData.otherCharacterImage;
        ChangeCharacterShown(dialogueData.textList[0].lineType == LineType.You);
    }

    public void ActivateNarratorBox(DialogueData newDialogueData)
    {
        dialogueData = newDialogueData;
        narratorBox.SetActive(true);
    }

    public void ShowNext(int currentDialogueLine)
    {
        dialogueText.text = string.Empty;
        if (currentDialogueLine <= dialogueData.textList.Length - 1)
        {
            ShowOnlyBox(dialogueData.textList[currentDialogueLine].lineType);
            if (dialogueData.textList[currentDialogueLine].lineType == LineType.You)
                nameText.text = "You";
            else
                nameText.text = dialogueData.otherCharacterName;
            ChangeCharacterShown(dialogueData.textList[currentDialogueLine].lineType == LineType.You);
        }
        else
        {
            nameText.text = dialogueData.otherCharacterName;
            ChangeCharacterShown(false, true);
        }
    }

    private void ShowOnlyBox(LineType lineType)
    {
        if (lineType == LineType.Narrator)
        {
            narratorBox.SetActive(true);
            dialogueBox.SetActive(false);
        }
        else
        {
            narratorBox.SetActive(false);
            dialogueBox.SetActive(true);
        }
    }

    private void ChangeCharacterShown(bool showMaincharacter, bool showAttributesBar = false)
    {
        if (showMaincharacter)
        {
            mainCharacterImageHolder.SetActive(true);
            otherCharacterImageHolder.SetActive(false);
        }
        else
        {
            mainCharacterImageHolder.SetActive(false);
            otherCharacterImageHolder.SetActive(true);
        }
        if (showAttributesBar)
        {
            minigameUI.Show(dialogueData);
        }
    }

    public void Hide()
    {
        narratorBox.SetActive(false);
        dialogueBox.SetActive(false);
        mainCharacterImageHolder.SetActive(false);
        otherCharacterImageHolder.SetActive(false);
    }
}
