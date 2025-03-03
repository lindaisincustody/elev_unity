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

    private void Start()
    {
        interactable.OnTriggerEnter += SetupFight;
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
        teleporter.Unlocked = true;
        teleporter.gameObject.SetActive(true);
        isFightComplete = true;
    }

    private void OnDestroy()
    {
        interactable.OnTriggerEnter -= SetupFight;
    }
}
