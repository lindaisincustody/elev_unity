using System;
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

    public Action<Enemy> OnSpawn { get; set; }
    public List<Enemy> Enemies = new();

    void Awake()
    {
        SpawnEnemies();
    }

    public void ResetSpawner()
    {
        foreach (Enemy enemy in Enemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        Enemies.Clear();
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnPoint.Count; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint[i].position, Quaternion.identity);
            newEnemy.name += "_" + i.ToString();
            newEnemy.minBound = minBound.position;
            newEnemy.maxBound = maxBound.position;

            OnSpawn?.Invoke(newEnemy);
            Enemies.Add(newEnemy);
            EnemyManager.Instance.RegisterEnemy(newEnemy);
        }
    }
}