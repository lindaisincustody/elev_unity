using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fight : MonoBehaviour
{
    [SerializeField] private List<SpawnEnemy> enemySpawners;
    [SerializeField] private FightInteractable interactable;
    [SerializeField] private Teleporter teleporter;
    [SerializeField] private bool startSetup;

    private bool isFightComplete;
    private int spawnersCompleted;

    private void Start()
    {
        interactable.OnTriggerEnter += SetupFight;

        foreach (SpawnEnemy spawner in enemySpawners)
        {
            spawner.OnSpawned += HandleSpawnerCompleted;
        }
    }

    private void HandleSpawnerCompleted()
    {
        spawnersCompleted++;

        if (spawnersCompleted >= enemySpawners.Count)
        {
            spawnersCompleted = 0; 
            OnEnemiesSpawned();
        }
    }

    private void OnEnemiesSpawned()
    {
        if (startSetup)
            SetupFight();
    }

    private void SetupFight()
    {
        List<Enemy> enemies = new();

        if (isFightComplete)
            foreach (SpawnEnemy spawner in enemySpawners)
            {
                spawner.ResetSpawner();
            }

        if (isFightComplete)
            isFightComplete = false;

        foreach (SpawnEnemy spawner in enemySpawners)
	    {
            foreach (Enemy enemy in spawner.Enemies)
            {
                enemies.Add(enemy);
            }
	    }

        FightManager.Instance.SetUpFight(this, enemies);
    }

    public void CompleteFight()
    {
        if (teleporter)
        {
            teleporter.Unlocked = true;
            teleporter.gameObject.SetActive(true);
        }

        isFightComplete = true;
    }

    private void OnDestroy()
    {
        interactable.OnTriggerEnter -= SetupFight;

        foreach (SpawnEnemy spawner in enemySpawners)
        {
            spawner.OnSpawned -= HandleSpawnerCompleted;
        }
    }
}
