using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordFiller : MonoBehaviour
{
    public TextMeshProUGUI[] wordText;
    private WordData wordDataHolder;
    [SerializeField] private TextMeshProUGUI poemText;
    // Start is called before the first frame update

    public void FillWords(WordData wordData)
    {
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
    }
}
