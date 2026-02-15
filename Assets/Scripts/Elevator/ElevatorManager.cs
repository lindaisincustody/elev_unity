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
    private GameObject npcUIPanel;

    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private Image npcSpriteImage;
    [SerializeField] private TextMeshProUGUI npcRequestText;

    [Header("NPC Data")] [SerializeField] private NPCData[] npcDataList;

    [Header("Settings")] [SerializeField] int totalFloors = 6;

    private int currentFloor = 0;
    private bool[] floorsCompleted;
    private NPCData currentNPC;

    void Start()
    {
        floorsCompleted = new bool[totalFloors];
        //SpawnNPCPassenger();
    }

    public void SpawnNPCPassenger()
    {
        if (npcDataList == null || npcDataList.Length == 0)
        {
            Debug.LogWarning("No NPC Data assigned to ElevatorManager.");
            return;
        }

        int index = Random.Range(0, npcDataList.Length);
        currentNPC = Instantiate(npcDataList[index]);

        int currentElevatorFloor = levels.GetCurrentLevel();

        int randomFloor = Random.Range(1, totalFloors + 1);
        while (randomFloor == currentElevatorFloor)
        {
            randomFloor = Random.Range(1, totalFloors + 1);
        }

        currentNPC.requestedFloor = randomFloor;
        levels.SetTargetLevel(currentNPC.requestedFloor);

        if (npcNameText != null)
            npcNameText.text = currentNPC.npcName;
        if (npcSpriteImage != null)
            npcSpriteImage.sprite = currentNPC.npcSprite;

        if (npcUIPanel != null)
            npcUIPanel.SetActive(true);
        if (npcRequestText != null)
        {
            npcRequestText.gameObject.SetActive(true);
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

    public void StartMiniGameForFloor(int floor)
    {
        if (currentNPC == null || floor != currentNPC.requestedFloor)
            return;

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

        SanityBar.instance.DecreaseSanity(50);

        if (SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
        {
            EndGame();
            yield break;
        }

        if (CameraElevatorShake.instance != null)
            CameraElevatorShake.instance.shakeIntensity = 0f;

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

        yield return new WaitForSeconds(1f);
        if (npcUIPanel != null)
            npcUIPanel.SetActive(false);


        leverMover.UnlockLever();

        SpawnNPCPassenger();
    }

    private void EndGame()
    {
        Debug.Log("Game Over: Player is in the underworld. Ending game and respawning...");

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector2(31.97f, 49.23f);
        }
        else
        {
            Debug.LogWarning("Player object not found!");
        }

        if (npcUIPanel != null)
            npcUIPanel.SetActive(false);

        if (leverMover != null)
            leverMover.enabled = false;

        if (levels != null)
            levels.enabled = false;

        if (movingCircle != null)
            movingCircle.SetActive(false);
        if (circle != null)
            circle.SetActive(false);
        if (fadeOut != null)
            fadeOut.gameObject.SetActive(false);

        if (CameraElevatorShake.instance != null)
            CameraElevatorShake.instance.shakeIntensity = 0f;

        this.enabled = false;
    }

    public bool IsFloorUnlocked(int floor)
    {
        return floorsCompleted[floor - 1];
    }
}