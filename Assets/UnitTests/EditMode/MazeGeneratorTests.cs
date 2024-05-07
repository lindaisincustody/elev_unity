using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using TMPro;
using NavMeshPlus.Components;

public class MazeGeneratorTests
{
    private MazeGenerator mazeGenerator;
    private NavMeshSurface navMeshSurface;
    private NavMeshCreator navMeshCreator;
    private CameraTransition cameraTransition;
    private GameObject endPrefab;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject to hold the MazeGenerator component
        GameObject gameObject = new GameObject();
        mazeGenerator = gameObject.AddComponent<MazeGenerator>();

        // Set up required components
        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        navMeshCreator = gameObject.AddComponent<NavMeshCreator>();
        cameraTransition = gameObject.AddComponent<CameraTransition>();
        endPrefab = new GameObject();

        // Assign references to MazeGenerator
        mazeGenerator.navmesh = navMeshSurface;
        mazeGenerator.creator = navMeshCreator;
        mazeGenerator.cameraTransition = cameraTransition;
        mazeGenerator.endPrefab = endPrefab;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(mazeGenerator.gameObject);
        Object.DestroyImmediate(endPrefab);
    }

    [Test]
    public void StartGeneratingMaze_CheckParameters()
    {
        // Arrange
        int mazeWidth = 10;
        int mazeHeight = 10;

        // Act
        mazeGenerator.StartGeneratingMaze(mazeWidth, mazeHeight);

        // Assert
        Assert.AreEqual(mazeWidth, mazeGenerator._mazeWidth);
        Assert.AreEqual(mazeHeight, mazeGenerator._mazeHeight);
    }
}
