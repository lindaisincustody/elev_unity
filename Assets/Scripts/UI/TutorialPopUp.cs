using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject BackgroundIamge;
    private float closeDelay = 3f;
    private Coroutine coroutine;
    private string boxText;

    private void Start()
    {
        boxText = text.text;    
    }

    public void ActivatePopUp()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (BackgroundIamge.activeSelf)
            return;

        BackgroundIamge.SetActive(true);

        StartCoroutine(TypeMessage(boxText));
    }

    private IEnumerator TypeMessage(string message)
    {
        text.text = ""; 
        text.gameObject.SetActive(true);
        string[] words = message.Split(' ');
        SoundManager.PlayLoopedSound(SoundManager.Sound.Typing);
        foreach (string word in words)
        {
            if (word.Contains("color"))
            {
                text.text += word + " ";
            }
            else
            {
                foreach (char letter in word)
                {
                    text.text += letter;
                    yield return new WaitForSeconds(0.01f);
                }
                text.text += " ";
            }
        }
        SoundManager.StopLoopedSound(SoundManager.Sound.Typing);
    }

    public void DeactivatePopUp()
    {
        coroutine = StartCoroutine(ClosePopUp());
    }

    private IEnumerator ClosePopUp()
    {
        yield return new WaitForSeconds(closeDelay);
        BackgroundIamge.SetActive(false);
        text.gameObject.SetActive(false);
    }
}
