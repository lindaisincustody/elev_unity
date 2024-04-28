using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using TMPro;

public class BookControllerTests 
{
    private BookController bookController;
    private GameObject poemObj;
    private GameObject wordsHolder;
    private TextMeshProUGUI poemText;

    [SetUp]
    public void Setup()
    {
        GameObject gameObject = new GameObject();
        bookController = gameObject.AddComponent<BookController>();
        poemObj = new GameObject();
        wordsHolder = new GameObject();
        poemText = poemObj.AddComponent<TextMeshProUGUI>();

        bookController.poemObj = poemObj;
        bookController.wordsHolder = wordsHolder;
        bookController.poemText = poemText;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(poemObj);
        Object.DestroyImmediate(wordsHolder);
        Object.DestroyImmediate(bookController.gameObject);
    }

    [Test]
    public void ExtendPoemAI_AddsPoemCorrectly()
    {
        string addedPoem = "New poem line";

        bookController.ExtendPoemAI(addedPoem);

        Assert.IsTrue(bookController.poemText.text.Contains(addedPoem));
    }

    [Test]
    public void ShowPoemAndWords_ShowsPoemAndWords()
    {
        bookController.ShowPoemAndWords();

        Assert.IsTrue(bookController.poemObj.activeSelf);
        Assert.IsTrue(bookController.wordsHolder.activeSelf);
    }

    [Test]
    public void HidePoemAndWords_HidesPoemAndWords()
    {
        bookController.ShowPoemAndWords();

        bookController.HidePoemAndWords();

        Assert.IsFalse(bookController.poemObj.activeSelf);
        Assert.IsFalse(bookController.wordsHolder.activeSelf);
    }

    [Test]
    public void ShowOldPoem_ActivatesOldPoem()
    {
        bookController.OldPoem = new GameObject();

        bookController.ShowOldPoem();

        Assert.IsTrue(bookController.OldPoem.activeSelf);
    }

    [Test]
    public void HideOldPoem_DeactivatesOldPoem()
    {
        bookController.OldPoem = new GameObject();
        bookController.ShowOldPoem();

        bookController.HideOldPoem();

        Assert.IsFalse(bookController.OldPoem.activeSelf);
    }
}
