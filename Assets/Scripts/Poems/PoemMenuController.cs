using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Trail
{
    Strength,
    Intelligence,
    Coordination,
    Neutral
}

public class PoemMenuController : MonoBehaviour
{
    public static PoemMenuController instance;
    [Header("Self-References")]
    [SerializeField] private WordFiller wordFiller;
    [SerializeField] private RectTransform bookMover;
    [SerializeField] private AutoFlip bookFlipper;
    [SerializeField] private RectTransform wordHolder;
    [SerializeField] private Image leftImage;
    [SerializeField] private PoemAI poemAI;
    [Header("Player References")]
    [SerializeField] private GameObject hero;
    [Header("Cursor References")]
    [SerializeField] CursorController cursor;
    [SerializeField] UIElementsHolder wordsElements;
    [Header("Parameters")]
    public AnimationCurve bookAnimationCurve;
    public float bookAnimationDuration = 1.0f;

    Player player;
    InputManager playerInput;
    PlayerMovement playerMovement;

    private int bookOffscreenPositionY = -1500;

    private bool _canBeTriggered = true;
    private bool isBookActive = false;
    private bool _canTurnPage = true;

    void Start()
    {
        instance = this;

        player = Player.instance;
        playerInput = player.GetInputManager;
        playerMovement = player.GetPlayerMovement;

        playerInput.OnInteract += OpenNextPage;
    }

    private void OnDestroy()
    {
        playerInput.OnInteract -= OpenNextPage;
    }

    public void OpenPoemBook(WordData wordsData)
    {
        if (!_canBeTriggered)
            return;

        InventoryUI.Instance.CanOpenInventory(false);
        poemAI.SendRequest(wordsData);
        isBookActive = true;
        _canBeTriggered = false;
        leftImage.enabled = false;
        playerMovement.SetMovement(false);

        for (int i = 0; i < wordsElements.cursorElements.Length; i++)
        {
            wordsElements.cursorElements[i].isCentered = false;
            wordsElements.cursorElements[i].cursorSpace = wordsData.words[i].wordLength;
        }

        Vector2 newOldWordPosition = wordsData.oldWordPosition;
        Vector2 newWordPosition = wordsData.WordPosition;
        wordHolder.anchoredPosition = newWordPosition;

        wordFiller.FillWords(wordsData);
        bookMover.gameObject.SetActive(true);
        bookMover.anchoredPosition = new Vector3(0, bookOffscreenPositionY, 0);
        StartCoroutine(MoveFromTo(new Vector3(0, bookOffscreenPositionY, 0), Vector3.zero, bookAnimationDuration));
        _canTurnPage = true;
    }

    public void ClosePoemBook()
    {
        isBookActive = false;
        StartCoroutine(MoveFromTo(Vector3.zero, new Vector3(0, bookOffscreenPositionY, 0), bookAnimationDuration));
        StartCoroutine(ClosePoemBookDelay());
    }

    IEnumerator MoveFromTo(Vector2 pointA, Vector2 pointB, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float fractionOfJourney = bookAnimationCurve.Evaluate(elapsedTime / duration);
            bookMover.anchoredPosition = Vector2.Lerp(pointA, pointB, fractionOfJourney);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bookMover.anchoredPosition = pointB;
    }

    private void OpenNextPage()
    {
        if (!isBookActive || !_canTurnPage)
            return;
        _canTurnPage = false;
        bookFlipper.FlipRightPage();
    }

    private IEnumerator ClosePoemBookDelay()
    {
        playerMovement.SetMovement(true);
        yield return new WaitForSeconds(2f);
        bookFlipper.FlipLeftPage();
        InventoryUI.Instance.CanOpenInventory(true);
        yield return new WaitForSeconds(3f);
        wordFiller.EnableWordChoosing(false);
        _canBeTriggered = true;
        bookMover.gameObject.SetActive(false);
    }

    public void UpdateAttributes(Word wordData)
    {
        player.AddGoldMultiplier(Attribute.Strength, wordData.strengthWeight);
        player.AddGoldMultiplier(Attribute.Coordination, wordData.coordinationWeight);
        player.AddGoldMultiplier(Attribute.Intelligence, wordData.intelligenceWeight);
        player.AddGoldMultiplier(Attribute.Neutrality, wordData.neutralWeight);
    }

    public void OnWritingPanelActivate()
    {
        StartCoroutine(EnableWordChoosing());
    }

    private IEnumerator EnableWordChoosing()
    {
        yield return new WaitForSeconds(1f);
        wordFiller.EnableWordChoosing(true);
        cursor.ActivateCursor(wordsElements.cursorElements, () => ExitPoem());
    }

    private void ExitPoem()
    {

    }
}
