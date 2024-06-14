using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutGameEndScreen : MonoBehaviour
{
    public Vector3 targetSize;
    public float animDuration;

    [SerializeField] InputManager playerInput;
    [Header("Screen Components")]
    [SerializeField] RectTransform expadingBackground;
    [SerializeField] RectTransform expandingLoseBG;
    [SerializeField] TextMeshProUGUI winText; 
    [SerializeField] TextMeshProUGUI loseText;
    [SerializeField] GameObject GoBackText;

    private float GoBackDelay = 2f;

    private void Awake()
    {
        playerInput.OnNext += ExitMinigame;
    }

    private void ExitMinigame()
    {
        if (GoBackText.activeSelf)
            SceneManager.LoadScene(DataManager.Instance.GetLastScene());
    }

    public void ShowWinScreen()
    {
        StartCoroutine(ExpandScreen(expadingBackground, winText));
    }

    public void ShowLoseScreen()
    {
        StartCoroutine(ExpandScreen(expandingLoseBG, loseText));
    }

    IEnumerator ExpandScreen(RectTransform background, TextMeshProUGUI text)
    {
        background.sizeDelta = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animDuration);
            background.sizeDelta = Vector3.Lerp(Vector3.zero, targetSize, t);

            yield return null;
        }

        background.sizeDelta = targetSize;
        StartCoroutine(ShowGoBackText());
        StartCoroutine(ShowEndText(text));
    }

    IEnumerator ShowEndText(TextMeshProUGUI text)
    {
        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        text.color = targetColor;
    }

    private void OnDestroy()
    {
        playerInput.OnNext -= ExitMinigame;
    }

    private IEnumerator ShowGoBackText()
    {
        yield return new WaitForSeconds(GoBackDelay);
        GoBackText.SetActive(true);
    }
}
