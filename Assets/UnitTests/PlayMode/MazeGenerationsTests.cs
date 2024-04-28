using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using NSubstitute;

public class MazeGenerationsTests
{
    private MazeGenerator mazeGenerator;

    [SetUp]
    public void SetUp()
    {
        // Create an empty game object
        GameObject gameObject = new GameObject();

        // Add components to the game object
        mazeGenerator = gameObject.AddComponent<MazeGenerator>();
        mazeGenerator.creator = gameObject.AddComponent<NavMeshCreator>();
    }

    [UnityTest]
    public IEnumerator MazeGeneratedCorrectly()
    {
        // Start generating a maze
        mazeGenerator.StartGeneratingMaze(10, 10);

        // Wait until the maze generation is complete
        yield return new WaitForSecondsRealtime(4f);

        // Check if the mazeGrid is initialized correctly
        Assert.IsNotNull(mazeGenerator.mazeGrid);
        Assert.AreEqual(10, mazeGenerator.mazeGrid.GetLength(0));
        Assert.AreEqual(10, mazeGenerator.mazeGrid.GetLength(1));

        // Check if all cells are instantiated
        foreach (var cell in mazeGenerator.mazeGrid)
        {
            Assert.IsNotNull(cell);
        }
    }
}

