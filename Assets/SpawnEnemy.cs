using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public Enemy enemyPrefab;

    [SerializeField] private List<Transform> spawnPoint;

    [Header("Bounds")]
    [SerializeField] private Transform minBound;
    [SerializeField] private Transform maxBound;

    void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        Vector2 offset;
        for (int i = 0; i < spawnPoint.Count; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint[i].position, Quaternion.identity);
            newEnemy.name += "_" + i.ToString();
            newEnemy.minBound = minBound.position;
            newEnemy.maxBound = maxBound.position;
        }
    }
}