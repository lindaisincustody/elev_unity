using System.Collections;
using UnityEngine;
using TMPro;
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
    [SerializeField] private SpriteRenderer fadeOut;

    [Header("NPC Passenger UI")] [SerializeField]
    private GameObject npcUIPanel; // Panel that displays NPC info

    [SerializeField] private TextMeshProUGUI npcNameText; // NPC name
    [SerializeField] private Image npcSpriteImage; // NPC sprite
    [SerializeField] private TextMeshProUGUI npcRequestText; // Greeting / Request text

    [Header("NPC Data")] [SerializeField] private NPCData[] npcDataList; // Array of NPC ScriptableObjects

    [Header("Settings")] [SerializeField] int totalFloors = 6; // Total floors in the elevator

    private int currentFloor = 0;
    private bool[] floorsCompleted;
    private NPCData currentNPC; // The NPC currently in the elevator

    void Start()
    {
        floorsCompleted = new bool[totalFloors];
        //SpawnNPCPassenger();
    }

    /// <summary>
    /// Chooses a new NPC from the npcDataList and shows the NPC UI.
    /// Overrides its requestedFloor to a random value between 1 and totalFloors that is not the current floor.
    /// </summary>
    public void SpawnNPCPassenger()
    {
        if (npcDataList == null || npcDataList.Length == 0)
        {
            Debug.LogWarning("No NPC Data assigned to ElevatorManager.");
            return;
        }

        // Choose one NPC at random and instantiate it so we can modify its requestedFloor.
        int index = Random.Range(0, npcDataList.Length);
        currentNPC = Instantiate(npcDataList[index]);

        // Get the current elevator floor from ElevatorLevels.
        int currentElevatorFloor = levels.GetCurrentLevel();

        // Choose a random requested floor between 1 and totalFloors that is not equal to currentElevatorFloor.
        int randomFloor = Random.Range(1, totalFloors + 1);
        while (randomFloor == currentElevatorFloor)
        {
            randomFloor = Random.Range(1, totalFloors + 1);
        }

        currentNPC.requestedFloor = randomFloor;
        levels.SetTargetLevel(currentNPC.requestedFloor);

        // Update UI with NPC info.
        if (npcNameText != null)
            npcNameText.text = currentNPC.npcName;
        if (npcSpriteImage != null)
            npcSpriteImage.sprite = currentNPC.npcSprite;

        // **Ensure that the NPC panel and text object are active before starting the coroutine.**
        if (npcUIPanel != null)
            npcUIPanel.SetActive(true);
        if (npcRequestText != null)
        {
            npcRequestText.gameObject.SetActive(true); // Activate the text object
            string message = currentNPC.greetingText + "\n(Take me to floor " + currentNPC.requestedFloor + "!)";
            NPCTypewriterWithSound typewriter = npcRequestText.GetComponent<NPCTypewriterWithSound>();
            if (typewriter != null)
            {
                typewriter.PlayTypewriterEffect(message);
            }
            else
            {
                npcRequestText.text = message;
            }
        }
    }


    /// <summary>
    /// Called when the elevator arrow reaches a floor.
    /// Only triggers the mini-game if the reached floor matches the NPC's requested floor.
    /// </summary>
    public void StartMiniGameForFloor(int floor)
    {
        // Only trigger if the reached floor matches the NPC's request.
        if (currentNPC == null || floor != currentNPC.requestedFloor)
            return;

        // Remove the check for floorsCompleted so that every NPC request triggers the mini-game.
        // if (floorsCompleted[floor - 1])
        // {
        //     leverMover.UnlockLever();
        //     return;
        // }

        // Lock the lever while the mini-game is active.
        leverMover.LockLever();
        StartCoroutine(ActivateMiniGame(floor));
    }

    private IEnumerator ActivateMiniGame(int floor)
    {
        // Fade in overlay.
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
        // Start the mini-game (assumed to be handled by your HollowCircleManager).
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

        // Mark the floor as served (if you still want to record this; if not, you can remove this line).
        floorsCompleted[floor - 1] = true;

        // Decrease sanity.
        SanityBar.instance.DecreaseSanityBy50();

        if (CameraElevatorShake.instance != null)
            CameraElevatorShake.instance.shakeIntensity = 0f;

        // Optionally, display thank-you text before hiding the UI.
        // Optionally, display thank-you text before hiding the UI.
        if (npcRequestText != null)
        {
            NPCTypewriterWithSound typewriter = npcRequestText.GetComponent<NPCTypewriterWithSound>();
            if (typewriter != null)
            {
                typewriter.PlayTypewriterEffect(currentNPC.thankYouText);
            }
            else
            {
                npcRequestText.text = currentNPC.thankYouText;
            }
        }


        // Hide NPC UI after a short delay.
        yield return new WaitForSeconds(1f);
        if (npcUIPanel != null)
            npcUIPanel.SetActive(false);

        // Unlock the lever.
        leverMover.UnlockLever();

        // Spawn next NPC passenger.
        SpawnNPCPassenger();
    }

    public bool IsFloorUnlocked(int floor)
    {
        return floorsCompleted[floor - 1];
    }
}