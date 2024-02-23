using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordFiller : MonoBehaviour
{
    public GameObject OldPoem;
    public GameObject OldChosenWord;
    public WritingEffect writingEffect;
    public ParticleMask particleMask;
    public Button[] wordButton;
    public TextMeshProUGUI[] wordText;
    [SerializeField] private TextMeshProUGUI poemText;

    [SerializeField] private TextMeshProUGUI OldpoemText;
    [SerializeField] CursorController cursor;

    private WordData wordDataHolder;
    private BookController bookController;
    private WordsListEffect wordsEffector;
    public bool firstPoem = true;

    private void Awake()
    {
        bookController = GetComponentInParent<BookController>();
        wordsEffector = GetComponentInParent<WordsListEffect>();
    }

    public void FillWords(WordData wordData)
    {
        if (!firstPoem)
        {
            OldpoemText.text = poemText.text;
            OldPoem.SetActive(true);
            OldChosenWord.SetActive(true);
        }

        wordDataHolder = wordData;
        poemText.text = wordData.Poem;
        for (int i = 0; i < 9; i++)
        {
            wordText[i].text = wordData.words[i].word;
        }

        foreach (Button btn in wordButton)
        {
            btn.interactable = true;
        }
    }

    public void ChooseWord(int wordIndex)
    {
        cursor.DeactivateCursor();
        foreach (Button btn in wordButton)
        {
            btn.interactable = false;
        }
        PoemMenuController.instance.UpdateAttributes(wordDataHolder.words[wordIndex]);
        OldChosenWord.GetComponent<TextMeshProUGUI>().text = wordDataHolder.words[wordIndex].word;
        OldChosenWord.GetComponent<RectTransform>().anchoredPosition = writingEffect.gameObject.GetComponent<RectTransform>().anchoredPosition - new Vector2(21.6f, 0);
        particleMask.calculateDuration(wordDataHolder.words[wordIndex].word);
        writingEffect.gameObject.SetActive(true);
        wordsEffector.FadeOutAllExcept(wordIndex);
        writingEffect.StartEffect(wordDataHolder.words[wordIndex].word, this);
    }

    public void StartClosingBook()
    {
        StartCoroutine(ClosingBookDelay());
    }

    private IEnumerator ClosingBookDelay()
    {
        yield return new WaitForSeconds(2f);
        bookController.InitiateClosingBook(writingEffect.gameObject);
    }
}
