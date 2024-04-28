using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutGameManagerTests
    {
    private CutGameManager cutGameManager;

    [SetUp]
    public void SetUp()
    {
        GameObject gameObject = new GameObject();
        // Find the CutGameManager in the scene
        cutGameManager = gameObject.AddComponent<CutGameManager>();
        cutGameManager.cutter = gameObject.AddComponent<GameobjectCutter>();
        cutGameManager.precisionText = gameObject.AddComponent<Scales>();
        cutGameManager.precisionText.ones = gameObject.AddComponent<NumberDisplayer>();
        cutGameManager.precisionText.tens = gameObject.AddComponent<NumberDisplayer>();
        cutGameManager.precisionText.hundreds = gameObject.AddComponent<NumberDisplayer>();
        cutGameManager.precisionText.thousands = gameObject.AddComponent<NumberDisplayer>();
        cutGameManager.precisionText.tenThousands = gameObject.AddComponent<NumberDisplayer>();
    }

    [UnityTest]
    public IEnumerator GameIsSetUp()
    {
        cutGameManager.SetUpGame();

        // Wait for one frame to ensure changes are applied
        yield return null;

        // Check if the Lose screen is active
        Assert.IsNotNull(cutGameManager.precisionText.tens);
    }

}