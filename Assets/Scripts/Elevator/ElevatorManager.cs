using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElevatorManager : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    CircleMovement circleMovement;

    [SerializeField] HollowCircleManager circleManager;
    [SerializeField] LeverMover leverMover;
    [SerializeField] ElevatorLevels levels;

    [Header("References")] [SerializeField]
    GameObject movingCircle;

    [SerializeField] GameObject circle;
    [SerializeField] SpriteRenderer fadeOut;

    [Header("Circle Game Levels To Beat")] [SerializeField]
    int totalFloors = 3;

    private int currentFloor = 0;
    private bool[] floorsCompleted;

    void Start()
    {
        floorsCompleted = new bool[totalFloors];
    }

    public void StartMiniGameForFloor(int floor)
    {
        if (floorsCompleted[floor - 1])
        {
            // Already completed, so no need to lock.
            return;
        }

        // Lock the lever to prevent further rotation while the mini-game is active.
        leverMover.LockLever();
        StartCoroutine(ActivateMiniGame(floor));
    }


    private IEnumerator ActivateMiniGame(int floor)
    {
        fadeOut.color = new Color(fadeOut.color.r, fadeOut.color.g, fadeOut.color.b, 0f);
        fadeOut.gameObject.SetActive(true);

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
        circleManager.ActivateGame(3, () => MiniGameComplete(floor));
        yield return new WaitForSeconds(0.5f);

        circleMovement.isActive = true;
    }

    private void MiniGameComplete(int floor)
    {
        StartCoroutine(DeactivateMiniGame(floor));
    }

    private IEnumerator DeactivateMiniGame(int floor)
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

        floorsCompleted[floor - 1] = true;

        // Unlock the lever now that the mini-game is complete.
        leverMover.UnlockLever();
    }


    public bool IsFloorUnlocked(int floor)
    {
        return floorsCompleted[floor - 1]; // Floors are 1-based
    }
}