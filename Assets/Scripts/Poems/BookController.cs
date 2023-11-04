using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public GameObject OldPoem;
    public GameObject OldWords;
    public WordFiller wordFiller;
    [SerializeField] private GameObject poemObj;
    [SerializeField] private GameObject wordsHolder;

    private Book book;
    private AutoFlip bookFlipper;
    private bool wordsWereShown = false;

    private void Awake()
    {
        book = GetComponent<Book>();
        bookFlipper = GetComponent<AutoFlip>();
    }

    public void ShowPoemAndWords()
    {
        if (wordsWereShown)
            return;
        wordsWereShown = true;
        poemObj.SetActive(true);
        wordsHolder.SetActive(true);
        book.interactable = false;
    }

    public void HidePoemAndWords()
    {
        poemObj.SetActive(false);
        wordsHolder.SetActive(false);
    }

    public void ShowOldPoem()
    {
        if (!OldPoem.activeSelf && !wordFiller.firstPoem)
        {
            OldPoem.SetActive(true);
            OldWords.SetActive(true);
        }
    }

    public void HideOldPoem()
    {
        if (OldPoem.activeSelf)
        {
            OldPoem.SetActive(false);
            OldWords.SetActive(false);
        }
    }

    public void InitiateClosingBook()
    {
        PoemMenuController.instance.ClosePoemBook();
        StartCoroutine(CloseBookDelay());
    }

    private IEnumerator CloseBookDelay()
    {
        yield return new WaitForSeconds(2f);
        wordFiller.firstPoem = false;
        book.interactable = true;
        wordsWereShown = false;
        HidePoemAndWords();
        bookFlipper.OpenFirstPage();
    }
}
