using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordFiller : MonoBehaviour
{
    public GameObject OldPoem;
    public GameObject OldWords;
    public WritingEffect writingEffect;
    public TextMeshProUGUI[] wordText;
    [SerializeField] private TextMeshProUGUI poemText;

    public TextMeshProUGUI[] OldWordText;
    [SerializeField] private TextMeshProUGUI OldpoemText;

    private WordData wordDataHolder;
    private BookController bookController;
    public bool firstPoem = true;

    private void Awake()
    {
        bookController = GetComponentInParent<BookController>();
    }

    public void FillWords(WordData wordData)
    {
        if (!firstPoem)
        {
            OldpoemText.text = poemText.text;
            for (int i = 0; i < 9; i++)
            {
                OldWordText[i].text = wordText[i].text;
            }
            OldPoem.SetActive(true);
            OldWords.SetActive(true);
        }

        wordDataHolder = wordData;
        poemText.text = wordData.Poem;
        for (int i = 0; i < 9; i++)
        {
            wordText[i].text = wordData.words[i].word;
        }
    }

    public void ChooseWord(int wordIndex)
    {
        PoemMenuController.instance.UpdateAttributes(wordDataHolder.words[wordIndex]);
        writingEffect.gameObject.SetActive(true);
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
