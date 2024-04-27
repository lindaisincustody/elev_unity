using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElevatorManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] CircleMovement circleMovement;
    [SerializeField] HollowCircleManager circleManager;
    [SerializeField] LeverMover leverMover;
    [SerializeField] ElevatorLevels levels;
    [SerializeField] HandMover handMover;

    [Header("References")]
    [SerializeField] GameObject movingCircle;
    [SerializeField] GameObject circle;
    [SerializeField] SpriteRenderer fadeOut;

    [Header("Circle Game Levels To Beat")]
    [SerializeField] int layers = 3;

    [Space(20)]
    [Header("End Screen")]
    [SerializeField] Animator endAnimator;
    [SerializeField] TextMeshProUGUI thanksText;

    private bool gameComplete = false;
 
    void Start()
    {
        handMover.SetActionOnHandReset(StartCircleGame);
        StartCircleGame();
    }

    private void StartCircleGame()
    {
        if (gameComplete)
        {
            StartCoroutine(Done());
            return;
        }

        levels.HighlightCurrentLevel();
        // If level is done, finish
        if (levels.GetCurrentLevel() == levels.GetTargetLevel())
        {
            gameComplete = true;
            leverMover.PullLever();
        }
        else
        {
            StartCoroutine(ActivateMiniGame());
        }
    }

    private IEnumerator Done()
    {
        yield return new WaitForSeconds(0.5f);
        endAnimator.SetTrigger("End");

        yield return new WaitForSeconds(2f);
        thanksText.alpha = 0f;

        // Turn on the text
        thanksText.gameObject.SetActive(true);
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        // Fade in
        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            thanksText.alpha = alpha;
            yield return null;
        }

        // Ensure the text reaches fully visible
        thanksText.alpha = 1f;
    }

    private void GameComplete()
    {
        StartCoroutine(DeactivateMiniGame());
    }

    private IEnumerator DeactivateMiniGame()
    {
        circleMovement.isActive = false;
        yield return new WaitForSeconds(1f);

        movingCircle.SetActive(false);
        circle.SetActive(false);

        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        float startAlpha = fadeOut.color.a;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            fadeOut.color = new Color(fadeOut.color.r, fadeOut.color.g, fadeOut.color.b, alpha);
            yield return null;
        }
        fadeOut.color = new Color(fadeOut.color.r, fadeOut.color.g, fadeOut.color.b, 0f);

        fadeOut.gameObject.SetActive(false);
        leverMover.ShiftRight(LevelGoUp);
    }

    private IEnumerator ActivateMiniGame()
    {
        fadeOut.color = new Color(fadeOut.color.r, fadeOut.color.g, fadeOut.color.b, 0f);
        fadeOut.gameObject.SetActive(true);

        // Fade in the SpriteRenderer
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 0.95f, elapsedTime / fadeDuration);
            fadeOut.color = new Color(fadeOut.color.r, fadeOut.color.g, fadeOut.color.b, alpha);
            yield return null;
        }

        fadeOut.color = new Color(fadeOut.color.r, fadeOut.color.g, fadeOut.color.b, 0.95f);

        yield return new WaitForSeconds(1f);
        movingCircle.SetActive(true);
        circle.SetActive(true);
        circleManager.ActivateGame(layers, GameComplete);
        yield return new WaitForSeconds(0.5f);
        circleMovement.isActive = true;
    }

    private void LevelGoUp()
    {
        levels.GoUp();
    }
}
