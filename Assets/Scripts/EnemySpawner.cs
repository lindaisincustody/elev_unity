using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Enemy enemyPrefab;

    [Space, SerializeField] private List<Transform> spawnPoints;

    [Header("Bounds")]
    [SerializeField] private Transform minBound;
    [SerializeField] private Transform maxBound;

    public List<Enemy> SpawnEnemies()
    {
        List<Enemy> spawnedEnemies = new List<Enemy>();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
            newEnemy.name = enemyPrefab.name + "_" + i;
            newEnemy.minBound = minBound.position;
            newEnemy.maxBound = maxBound.position;

            EnemyManager.Instance.RegisterEnemy(newEnemy);

            spawnedEnemies.Add(newEnemy);
        }

        return spawnedEnemies;
    }
}
