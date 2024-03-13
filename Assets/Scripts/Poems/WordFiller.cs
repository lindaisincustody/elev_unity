using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordFiller : MonoBehaviour
{
    public GameObject OldPoem;
    public WritingEffect writingEffect;
    public ParticleMask particleMask;
    public Button[] wordButton;
    public TextMeshProUGUI[] wordText;
    public SelectedWordWritingEffect[] writingEffectController;
    [SerializeField] private TextMeshProUGUI poemText;

    [SerializeField] private TextMeshProUGUI OldpoemText;
    [SerializeField] CursorController cursor;

    private WordData wordDataHolder;
    private BookController bookController;
    private WordsListEffect wordsEffector;
    public bool firstPoem = true;
    private string oldChosenWord;
    private bool canChooseWord = false;

    private void Awake()
    {
        bookController = GetComponentInParent<BookController>();
        wordsEffector = GetComponentInParent<WordsListEffect>();
    }

    public void FillWords(WordData wordData)
    {
        if (!firstPoem)
        {
            OldpoemText.text = poemText.text.Replace("_____", oldChosenWord);
            OldPoem.SetActive(true);
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
        if (!canChooseWord)
            return;
        writingEffectController[wordIndex].SelectedWordEffect();
        cursor.DeactivateCursor();
        foreach (Button btn in wordButton)
        {
            btn.interactable = false;
        }
        PoemMenuController.instance.UpdateAttributes(wordDataHolder.words[wordIndex]);
        particleMask.calculateDuration(wordDataHolder.words[wordIndex].word);
        oldChosenWord = wordDataHolder.words[wordIndex].word;
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

    public void EnableWordChoosing(bool canChoose)
    {
        canChooseWord = canChoose;
    }
}
