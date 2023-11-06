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
    public TextMeshProUGUI[] wordText;
    [SerializeField] private TextMeshProUGUI poemText;

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
            OldPoem.SetActive(true);
            OldChosenWord.SetActive(true);
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
        OldChosenWord.GetComponent<TextMeshProUGUI>().text = wordDataHolder.words[wordIndex].word;
        OldChosenWord.GetComponent<RectTransform>().anchoredPosition = writingEffect.gameObject.GetComponent<RectTransform>().anchoredPosition - new Vector2(21.6f, 0);
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
