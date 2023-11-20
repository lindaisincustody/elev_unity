using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordsListEffect : MonoBehaviour
{
    public TextMeshProUGUI[] textObjects; 
    public TextMeshProUGUI poem;
    public float fadeDuration = 2f;

    private void OnEnable()
    {
        FadeInAllText();
    }

    private void FadeInAllText()
    {
        foreach (TextMeshProUGUI textObject in textObjects)
        {
            StartCoroutine(FadeTextToFullAlpha(textObject, fadeDuration));
        }
        StartCoroutine(FadeTextToFullAlpha(poem, fadeDuration));
    }

    public void FadeOutAllExcept(int index)
    {
        for (int i = 0; i < textObjects.Length; i++)
        {
            if (i != index)
            {
                StartCoroutine(FadeTextToZeroAlpha(textObjects[i], fadeDuration));
            }
        }
    }


    private IEnumerator FadeTextToFullAlpha(TextMeshProUGUI textObject, float time)
    {
        textObject.gameObject.SetActive(true);
        textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0);
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / time);
            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeTextToZeroAlpha(TextMeshProUGUI textObject, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - elapsedTime / time);
            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, alpha);
            yield return null;
        }
        textObject.gameObject.SetActive(false);
    }
}
