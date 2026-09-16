using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Item item;

    [Space, SerializeField] private List<Transform> spawnPoints;

    [Header("Bounds")]
    [SerializeField] private Transform minBound;
    [SerializeField] private Transform maxBound;
    [SerializeField] private EnemyGlyphSO enemyGlyphSO;

    public int EnemyCount => spawnPoints.Count;

    public List<Enemy> SpawnEnemies(int count)
    {
        List<Enemy> spawnedEnemies = new List<Enemy>();

        for (int i = 0; i < count; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
            newEnemy.name = enemyPrefab.name + "_" + i;
            newEnemy.minBound = minBound.position;
            newEnemy.maxBound = maxBound.position;
            newEnemy.GenerateRandomSymbols(enemyGlyphSO.Labels);

            EnemyManager.Instance.RegisterEnemy(newEnemy);
            newEnemy.OnDeath += OnDeath;

            spawnedEnemies.Add(newEnemy);
        }

        return spawnedEnemies;
    }

    private void OnDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnDeath;

        ItemDropper.Instance.Drop(item, enemy.transform.position);
    }
}
