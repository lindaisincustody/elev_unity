using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MazeScreen : MonoBehaviour
{
    public Vector3 targetSize;
    public float animDuration;

    public float blinkMin = 0.1f;
    public float blinkMax = 2f;

    [SerializeField] InputManager playerInput;
    [Header("Screen Components")]
    [SerializeField] RectTransform expadingBackground;
    [SerializeField] Image expandingBackgroundImage;
    [SerializeField] RectTransform mask;
    [SerializeField] Image loseTextbg;
    [SerializeField] Image loseText;
    [SerializeField] Material textMaterial;
    [SerializeField] GameObject GoBackText;
    [SerializeField] List<EyesAnimation> eyes = new List<EyesAnimation>();

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

    public void ShowEndScreen()
    {
        StartCoroutine(ExpandScreen(expadingBackground));
    }
    
    private IEnumerator StartBlinking()
    {
        float time = Random.Range(blinkMin, blinkMax);
        int index = Random.Range(0, eyes.Count);
        eyes[index].Blink();
        yield return new WaitForSeconds(time);
        StartCoroutine(StartBlinking());
    }

    private IEnumerator ShowGoBackText()
    {
        yield return new WaitForSeconds(GoBackDelay);
        GoBackText.SetActive(true);
    }


    IEnumerator ExpandScreen(RectTransform background)
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
        expandingBackgroundImage.color = Color.white;
        mask.sizeDelta = Vector3.one;
        StartCoroutine(ShowGoBackText());
        StartCoroutine(ShowLoseText());
        StartCoroutine(ShowVignette());
        StartCoroutine(StartBlinking());
    }

    IEnumerator ShowLoseText()
    {
        Color startColor = loseText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            loseText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            loseTextbg.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        loseText.color = targetColor;
        loseTextbg.color = targetColor;
    }

    IEnumerator ShowVignette()
    {
        float startIntensity = 0f;
        float targetIntensity = 0.77f;
        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
            textMaterial.SetFloat("_VignetteIntensity", intensity);
            yield return null;

            elapsedTime += Time.deltaTime;
        }
        textMaterial.SetFloat("_VignetteIntensity", targetIntensity);
    }

    private void OnDestroy()
    {
        textMaterial.SetFloat("_VignetteIntensity", 0f);
        playerInput.OnNext -= ExitMinigame;
    }
}
