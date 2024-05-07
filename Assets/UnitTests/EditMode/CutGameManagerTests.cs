using UnityEngine;
using NUnit.Framework;
using UnityEngine.UI;
using UnityEngine.TestTools; // For coroutine support in tests

public class CutGameManagerTests
{
    private GameObject gameObject;
    private CutGameManager cutGameManager;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        cutGameManager = gameObject.AddComponent<CutGameManager>();

        // Setting up the required components and references
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<Canvas>();
        cutGameManager.fruitImagePrefab = new GameObject().AddComponent<Image>();
        cutGameManager.fruitImageHolder = new GameObject("FruitImageHolder").transform;
        cutGameManager.objectsHolder = new GameObject("ObjectsHolder").transform;
        cutGameManager.endScreen = new GameObject().AddComponent<CutGameEndScreen>();
        cutGameManager.cutter = new GameObject().AddComponent<GameobjectCutter>();
        cutGameManager.calculator = new GameObject().AddComponent<WeightCalculator>();

        // Set up the Scales component with its NumberDisplayers
        GameObject scalesObject = new GameObject("PrecisionText");
        cutGameManager.precisionText = scalesObject.AddComponent<Scales>();
        SetupNumberDisplayers(cutGameManager.precisionText);

        // Additional setup for components that require it
        cutGameManager.fruitsList = ScriptableObject.CreateInstance<FruitSpritesData>();
        // Assume FruitSpritesData is properly initialized here if necessary

        // Initialize the game for testing conditions
        cutGameManager.SetUpGame();
    }

    private void SetupNumberDisplayers(Scales scales)
    {
        scales.ones = CreateNumberDisplayer("Ones");
        scales.tens = CreateNumberDisplayer("Tens");
        scales.hundreds = CreateNumberDisplayer("Hundreds");
        scales.thousands = CreateNumberDisplayer("Thousands");
        scales.tenThousands = CreateNumberDisplayer("TenThousands");
    }

    private NumberDisplayer CreateNumberDisplayer(string name)
    {
        GameObject displayerObject = new GameObject(name);
        NumberDisplayer displayer = displayerObject.AddComponent<NumberDisplayer>();

        // Initialize the SpriteRenderers for each NumberDisplayer
        displayer.linesColor = new SpriteRenderer[7]; // Assuming there are 7 segments in a typical digital number display
        for (int i = 0; i < displayer.linesColor.Length; i++)
        {
            GameObject line = new GameObject($"Line{i}");
            line.transform.parent = displayerObject.transform;
            displayer.linesColor[i] = line.AddComponent<SpriteRenderer>();
        }

        // Initialize the lines array if necessary
        displayer.lines = new GameObject[7];
        for (int i = 0; i < displayer.lines.Length; i++)
        {
            displayer.lines[i] = new GameObject($"Segment{i}");
        }

        return displayer;
    }



    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(gameObject);
    }

    [Test]
    public void OnCut_UpdatesPrecisionAndHandlesGameProgress()
    {
        // Set up the test conditions
        cutGameManager.totalFruitsToSpawn = 5;
        cutGameManager.spawnedFruits = 4;
        cutGameManager.calculator.SetWeightDifference(5f);
        cutGameManager.precision = 10f;

        // Act by simulating a cut
        cutGameManager.OnCut();

        // Assert that the color is updated correctly and that we check for win condition
        Assert.AreEqual(cutGameManager.winColor, cutGameManager.precisionText.GetCurrentColor());
        // We assume GetCurrentColor() is a method to get the current color of the text
    }
}
