using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathShowersSpawner : MonoBehaviour
{
    [Header("Path Shower")]
    [SerializeField] private PathConsumable pathConsumable;

    private int spawnerToSpawn = 1;

    private int _mazeWidth;
    private int _mazeHeight;
    private int cellSize = 4;

    public void SetMazeParameters(int mazeWidth, int mazeHeight, int mazeCellSize)
    {
        _mazeHeight = mazeHeight;
        _mazeWidth = mazeWidth;
        cellSize = mazeCellSize;
    }

    public void SpawnPathShowers()
    {
        List<Vector2> spawnPositions = new List<Vector2>();

        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeHeight; j++)
            {
                spawnPositions.Add(new Vector2(i * cellSize, j * cellSize));
            }
        }

        int numToSpawn = Mathf.Min(spawnerToSpawn, spawnPositions.Count);
        for (int i = 0; i < numToSpawn; i++)
        {
            int randomIndex = Random.Range(0, spawnPositions.Count);
            Vector2 spawnPosition = spawnPositions[randomIndex];
            spawnPositions.RemoveAt(randomIndex);

            PathConsumable newConsumable = Instantiate(pathConsumable, spawnPosition, Quaternion.identity);
            newConsumable.transform.parent = gameObject.transform;
        }
    }

    public void IncreasePathShowerCount(int additionalCount)
    {
        spawnerToSpawn += additionalCount;
    }
}
