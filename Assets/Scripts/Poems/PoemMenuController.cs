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

    public Attributes heroAttributes;
    private int numberOfPoems;

    // Start is called before the first frame update
    void Start()
    {
        numberOfPoems = FindObjectsOfType<PoemTrigger>().Length;
        heroAttributes = hero.GetComponent<Attributes>();
        instance = this;
    }

    public void OpenPoemBook(WordData wordsData)
    {
        bookPanel.SetActive(true);
        wordFiller.FillWords(wordsData);
    }

    public void UpdateAttributes(Word wordData)
    {
        heroAttributes.heroStrength += wordData.strengthWeight;
        heroAttributes.heroIntelligence += wordData.intelligenceWeight;
        heroAttributes.heroCoordination += wordData.coordinationWeight;
        heroAttributes.heroNeutrality += wordData.neutralWeight;
        displayer.UpdateText();
        bookPanel.SetActive(false);
    }

    public void ShowFightOptions()
    {
        skillTree.UpdateSkilltree(numberOfPoems);
        skilltreePanel.SetActive(true);
    }
}
