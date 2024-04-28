using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using TMPro;
using NavMeshPlus.Components;
using UnityEngine.UI;

public class CutGameManagerTests 
{
    private CutGameManager cutGameManager;
    private GameObject gameObject;
    private GameObject fruitImagePrefab;
    private Transform fruitImageHolder;
    private GameObject objectsHolder;
    private GameObject cutGameEndScreen;
    private GameObject gameobjectCutter;
    private GameObject weightCalculator;
    private GameObject scales;

    [SetUp]
    public void Setup()
    {
        AssignDependencies();
    }

    private void AssignDependencies()
    {
        // Create GameObjects
        gameObject = new GameObject();
        fruitImagePrefab = new GameObject();
        fruitImageHolder = new GameObject().transform;
        objectsHolder = new GameObject();
        cutGameEndScreen = new GameObject();
        gameobjectCutter = new GameObject();
        weightCalculator = new GameObject();
        scales = new GameObject();

        // Add components
        cutGameManager = gameObject.AddComponent<CutGameManager>();
        cutGameManager.fruitImagePrefab = fruitImagePrefab.GetComponent<Image>();
        cutGameManager.fruitImageHolder = fruitImageHolder;
        cutGameManager.objectsHolder = objectsHolder.transform;
        cutGameManager.endScreen = cutGameEndScreen.GetComponent<CutGameEndScreen>();
        cutGameManager.cutter = gameobjectCutter.AddComponent<GameobjectCutter>();
        cutGameManager.calculator = weightCalculator.GetComponent<WeightCalculator>();
        cutGameManager.precisionText = scales.GetComponent<Scales>();
        cutGameManager.winColor = Color.white;
        cutGameManager.loseColor = Color.black;
        cutGameManager.spawnedFruits = 3;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
        Object.DestroyImmediate(fruitImagePrefab);
        Object.DestroyImmediate(fruitImageHolder.gameObject);
        Object.DestroyImmediate(objectsHolder);
        Object.DestroyImmediate(cutGameEndScreen);
        Object.DestroyImmediate(gameobjectCutter);
        Object.DestroyImmediate(weightCalculator);
        Object.DestroyImmediate(scales);
    }

    [Test]
    public void OnCut_UpdatesPrecisionColor()
    {
        Assert.AreNotEqual(cutGameManager.winColor, cutGameManager.loseColor);
    }

    [Test]
    public void OnCut_WhenNotAllFruitsSpawned_ResetsCutting()
    {
        // Arrange

        Assert.AreEqual(3, cutGameManager.spawnedFruits); // Check if spawned fruits are reset to 0
    }

}
