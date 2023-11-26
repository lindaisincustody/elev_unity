using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private WordFiller wordFiller;
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private GameObject skilltreePanel;
    [SerializeField] private GameObject hero;
    [SerializeField] private SkillTree skillTree;
    [SerializeField] private AttributesDisplayer displayer;
    [SerializeField] private RectTransform oldWord;
    [SerializeField] private RectTransform wordHolder;
    public AnimationCurve bookAnimationCurve;
    public float bookAnimationDuration = 1.0f;

    private RectTransform bookMover;
    private int bookOffscreenPositionY = -1500;
    private Vector2 OldWordPosition;

    public Attributes heroAttributes;
    private int numberOfPoems;
    private bool _canBeTriggered = true;

    // Start is called before the first frame update
    void Start()
    {
        numberOfPoems = FindObjectsOfType<PoemTrigger>().Length;
        heroAttributes = hero.GetComponent<Attributes>();
        instance = this;
        bookMover = bookPanel.GetComponent<RectTransform>();

    }

    public void OpenPoemBook(WordData wordsData)
    {
        if (!_canBeTriggered)
            return;
        _canBeTriggered = false;

        Vector2 newOldWordPosition = wordsData.oldWordPosition;
        Vector2 newWordPosition = wordsData.WordPosition;
        wordHolder.anchoredPosition = newWordPosition;
        if (OldWordPosition != null)
        {
            oldWord.anchoredPosition = OldWordPosition;
        }
        OldWordPosition = newOldWordPosition;

        wordFiller.FillWords(wordsData);
        bookPanel.SetActive(true);
        bookMover.anchoredPosition = new Vector3(0, bookOffscreenPositionY, 0);
        StartCoroutine(MoveFromTo(new Vector3(0, bookOffscreenPositionY, 0), Vector3.zero, bookAnimationDuration));
    }

    public void ClosePoemBook()
    {
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


    private IEnumerator ClosePoemBookDelay()
    {
        yield return new WaitForSeconds(4f);
        _canBeTriggered = true;
        bookPanel.SetActive(false);
    }

    public void UpdateAttributes(Word wordData)
    {
        heroAttributes.heroStrength += wordData.strengthWeight;
        heroAttributes.heroIntelligence += wordData.intelligenceWeight;
        heroAttributes.heroCoordination += wordData.coordinationWeight;
        heroAttributes.heroNeutrality += wordData.neutralWeight;
        displayer.UpdateText();
    }

    public void ShowFightOptions()
    {
        skillTree.UpdateSkilltree(numberOfPoems);
        skilltreePanel.SetActive(true);
    }

    public void HideFightOptions()
    {
        skilltreePanel.SetActive(false);
    }
}
