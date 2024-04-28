using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using TMPro;


public class DialogueControllerTests 
{
    [Test]
    public void ShowPoemAndWords_ActivatesPoemAndWords()
    {
        // Arrange
        var gameObject = new GameObject();
        var controller = gameObject.AddComponent<BookController>();
        var poemObj = new GameObject();
        var wordsHolder = new GameObject();

        controller.poemObj = poemObj;
        controller.wordsHolder = wordsHolder;

        // Act
        controller.ShowPoemAndWords();

        // Assert
        Assert.IsTrue(controller.poemObj.activeSelf);
        Assert.IsTrue(controller.wordsHolder.activeSelf);
    }

    [Test]
    public void HidePoemAndWords_DeactivatesPoemAndWords()
    {
        // Arrange
        var gameObject = new GameObject();
        var controller = gameObject.AddComponent<BookController>();
        var poemObj = new GameObject();
        var wordsHolder = new GameObject();

        controller.poemObj = poemObj;
        controller.wordsHolder = wordsHolder;

        // Act
        controller.HidePoemAndWords();

        // Assert
        Assert.IsFalse(controller.poemObj.activeSelf);
        Assert.IsFalse(controller.wordsHolder.activeSelf);
    }

    [Test]
    public void ShowOldPoem_ActivatesOldPoem()
    {
        // Arrange
        var gameObject = new GameObject();
        var controller = gameObject.AddComponent<BookController>();
        var oldPoem = new GameObject();
        controller.OldPoem = oldPoem;

        // Act
        controller.ShowOldPoem();

        // Assert
        Assert.IsTrue(controller.OldPoem.activeSelf);
    }

    [Test]
    public void HideOldPoem_DeactivatesOldPoem()
    {
        // Arrange
        var gameObject = new GameObject();
        var controller = gameObject.AddComponent<BookController>();
        var oldPoem = new GameObject();
        controller.OldPoem = oldPoem;

        // Act
        controller.HideOldPoem();

        // Assert
        Assert.IsFalse(controller.OldPoem.activeSelf);
    }
}
