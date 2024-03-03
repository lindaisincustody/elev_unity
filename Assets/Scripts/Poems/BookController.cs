using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookController : MonoBehaviour
{
    public GameObject OldPoem;
    public GameObject OldChosenWord;
    public WordFiller wordFiller;
    [SerializeField] private GameObject poemObj;
    [SerializeField] private GameObject wordsHolder;
    [SerializeField] private TextMeshProUGUI poemText;

    private Book book;
    private AutoFlip bookFlipper;
    private bool wordsWereShown = false;

    private void Awake()
    {
        book = GetComponent<Book>();
        bookFlipper = GetComponent<AutoFlip>();
    }

    public void ExtendPoemAI(string addedPoem)
    {
        poemText.text += "\n "+ addedPoem;
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
            OldChosenWord.SetActive(true);
        }
    }

    public void HideOldPoem()
    {
        if (OldPoem.activeSelf)
        {
            OldPoem.SetActive(false);
            OldChosenWord.SetActive(false);
        }
    }

    public void InitiateClosingBook(GameObject objectToDisable)
    {
        PoemMenuController.instance.ClosePoemBook();
        StartCoroutine(CloseBookDelay(objectToDisable));
    }

    private IEnumerator CloseBookDelay(GameObject objectToDisable)
    {
        yield return new WaitForSeconds(2f);
        wordFiller.firstPoem = false;
        wordsWereShown = false;
        HidePoemAndWords();
        objectToDisable.SetActive(false);
    }
}
