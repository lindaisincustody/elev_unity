using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI
{
    private GameObject dialogueBox;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI nameText;
    private Image mainCharacterImage;
    private Image otherCharacterImage;
    private GameObject mainCharacterImageHolder;
    private GameObject otherCharacterImageHolder;

    private DialogueData dialogueData;
    private MinigameUI minigameUI;

    public DialogueUI(GameObject box, TextMeshProUGUI text, TextMeshProUGUI name, Image mainImage, Image otherImage, GameObject mainHolder, GameObject otherHolder, MinigameUI minigame)
    {
        dialogueBox = box;
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
        if (dialogueData.textList == null)
        {
            newDialogueData = ScriptableObject.CreateInstance<DialogueData>();
            newDialogueData.otherCharacterName = "Character Name";
            newDialogueData.textList = new DialogueData.CharacterData[]
            {
            new DialogueData.CharacterData { isYourText = true, dialogueLineText = "Hello!" },
            new DialogueData.CharacterData { isYourText = false, dialogueLineText = "Hi there!" }
            };
            newDialogueData.activateFight = false;
            // Assume Sprite assets exist and are assigned accordingly
            newDialogueData.mainCharacterImage = CreateSprite(Color.white);
            newDialogueData.otherCharacterImage = CreateSprite(Color.red);
            dialogueData = newDialogueData;
        }
        ChangeCharacterShown(dialogueData.textList[0].isYourText);
    }

    public Sprite CreateSprite(Color color, int width = 100, int height = 100)
    {
        // Create a new texture with the specified color
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

        // Create a new sprite from the texture
        Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        return newSprite;
    }

    public void ShowNext(int currentDialogueLine)
    {
        dialogueText.text = string.Empty;
        if (currentDialogueLine <= dialogueData.textList.Length - 1)
        {
            if (dialogueData.textList[currentDialogueLine].isYourText)
                nameText.text = "You";
            else
                nameText.text = dialogueData.otherCharacterName;
            ChangeCharacterShown(dialogueData.textList[currentDialogueLine].isYourText);
        }
        else
        {
            nameText.text = dialogueData.otherCharacterName;
            ChangeCharacterShown(false, true);
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
        dialogueBox.SetActive(false);
        mainCharacterImageHolder.SetActive(false);
        otherCharacterImageHolder.SetActive(false);
    }
}
