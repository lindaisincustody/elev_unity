using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorManager : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    private CircleMovement circleMovement;

    [SerializeField] private HollowCircleManager circleManager;
    [SerializeField] private LeverMover leverMover;
    [SerializeField] private ElevatorLevels levels;

    [Header("References")] [SerializeField]
    private GameObject movingCircle;

    [SerializeField] private GameObject circle;
    [SerializeField] private Image fadeOut;

    [Header("NPC Passenger UI")] [SerializeField]
    private GameObject npcUIPanel;

    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private Image npcSpriteImage;
    [SerializeField] private TextMeshProUGUI npcRequestText;

    [Header("Settings")] [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float fadeAlpha = 0.95f;
    [SerializeField] private float beatDuration = 1f;
    [SerializeField] private float miniGameStartDelay = 0.5f;

    private NPCData passenger;
    private UniTaskCompletionSource floorReached;

    public async UniTask Ride(NPCData newPassenger, int miniGameLevels, CancellationToken token)
    {
        passenger = newPassenger;
        floorReached = new UniTaskCompletionSource();

        levels.ResetToGround();
        levels.SetTargetLevel(passenger.requestedFloor);
        ShowPassenger();
        leverMover.UnlockLever();

        await floorReached.Task.AttachExternalCancellation(token);

        leverMover.LockLever();

        SetFadeAlpha(0f);
        fadeOut.gameObject.SetActive(true);

        await FadeTo(fadeAlpha, token);
        await UniTask.Delay(TimeSpan.FromSeconds(beatDuration), cancellationToken: token);

        await PlayMiniGame(miniGameLevels, token);

        await FadeTo(0f, token);
        fadeOut.gameObject.SetActive(false);

        Say(passenger.thankYouText);
        await UniTask.Delay(TimeSpan.FromSeconds(beatDuration), cancellationToken: token);

        npcUIPanel.SetActive(false);
    }

    public void OnFloorReached(int floor)
    {
        if (floor != passenger.requestedFloor)
            return;

        floorReached.TrySetResult();
    }

    public void Stop()
    {
        leverMover.LockLever();

        circleMovement.isActive = false;
        circleManager.ClearCircles();

        movingCircle.SetActive(false);
        circle.SetActive(false);
        npcUIPanel.SetActive(false);

        SetFadeAlpha(0f);
        fadeOut.gameObject.SetActive(false);
    }

    private async UniTask PlayMiniGame(int miniGameLevels, CancellationToken token)
    {
        movingCircle.SetActive(true);
        circle.SetActive(true);

        UniTaskCompletionSource miniGameCompleted = new UniTaskCompletionSource();
        circleManager.ActivateGame(miniGameLevels, () => miniGameCompleted.TrySetResult());

        await UniTask.Delay(TimeSpan.FromSeconds(miniGameStartDelay), cancellationToken: token);
        circleMovement.isActive = true;

        await miniGameCompleted.Task.AttachExternalCancellation(token);

        circleMovement.isActive = false;
        await UniTask.Delay(TimeSpan.FromSeconds(beatDuration), cancellationToken: token);

        movingCircle.SetActive(false);
        circle.SetActive(false);
    }

    private void ShowPassenger()
    {
        npcNameText.text = passenger.npcName;
        npcSpriteImage.sprite = passenger.npcSprite;

        npcUIPanel.SetActive(true);
        npcRequestText.gameObject.SetActive(true);

        Say(passenger.greetingText + "\n(Take me to floor " + passenger.requestedFloor + "!)");
    }

    private void Say(string message)
    {
        npcRequestText.GetComponent<NPCTypewriterWithSound>().PlayTypewriterEffect(message);
    }

    private async UniTask FadeTo(float targetAlpha, CancellationToken token)
    {
        float startAlpha = fadeOut.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetFadeAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration));

            await UniTask.Yield(token);
        }

        SetFadeAlpha(targetAlpha);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeOut.color;
        fadeOut.color = new Color(color.r, color.g, color.b, alpha);
    }
}
