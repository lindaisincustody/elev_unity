using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameUI
{
    private GameObject minigamesBox;
    private GameObject multiplierFrame;
    private Image strengthImage;
    private Image intelligenceImage;
    private Image coordinationImage;
    private Image neutralityImage;
    public bool isActive = false;

    public MinigameUI(GameObject bar, GameObject frame, Image strength, Image intelligence, Image coordination, Image neutrality)
    {
        minigamesBox = bar;
        multiplierFrame = frame;
        strengthImage = strength;
        intelligenceImage = intelligence;
        coordinationImage = coordination;
        neutralityImage = neutrality;
    }

    public void Show(DialogueData data)
    {
        isActive = true;
        minigamesBox.SetActive(true);
        multiplierFrame.SetActive(true);
        strengthImage.fillAmount = data.strengthGameCoinsMultiplier / 4 - 0.25f;
        intelligenceImage.fillAmount = data.intelligenceGameCoinsMultiplier / 4 - 0.25f;
        coordinationImage.fillAmount = data.coordinationGameCoinsMultiplier / 4 - 0.25f;
        neutralityImage.fillAmount = data.neutralityGameCoinsMultiplier / 4 - 0.25f;
    }

    public void Hide()
    {
        minigamesBox.SetActive(false);
        multiplierFrame.SetActive(false);
    }
}
