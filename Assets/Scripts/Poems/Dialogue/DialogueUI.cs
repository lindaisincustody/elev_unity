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
    private GameObject mainCharacterImageHolder;

    private DialogueData dialogueData;

    public DialogueUI(GameObject box, GameObject narratorbox, TextMeshProUGUI text, TextMeshProUGUI name, Image mainImage, GameObject mainHolder)
    {
        dialogueBox = box;
        narratorBox = narratorbox;
        dialogueText = text;
        nameText = name;
        mainCharacterImage = mainImage;
        mainCharacterImageHolder = mainHolder;
    }

    public void ActivateDialogueBox(DialogueData newDialogueData)
    {
        dialogueData = newDialogueData;
        dialogueBox.SetActive(true);
        mainCharacterImage.sprite = dialogueData.mainCharacterImage;
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
            ShowOnlyBox(LineType.Enemy);
            nameText.text = dialogueData.otherCharacterName;
            ChangeCharacterShown(false);
        }
    }

    private void ChangeCharacterShown(bool showMaincharacter)
    {
        if (showMaincharacter)
        {
            mainCharacterImage.sprite = dialogueData.mainCharacterImage;
        }
        else
        {
            mainCharacterImage.sprite = dialogueData.otherCharacterImage;
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


    public void Hide()
    {
        narratorBox.SetActive(false);
        dialogueBox.SetActive(false);
        mainCharacterImageHolder.SetActive(false);
    }
}
