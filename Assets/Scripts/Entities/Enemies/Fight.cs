using System.Collections.Generic;
using UnityEngine;

public class Fight : MonoBehaviour
{
    [SerializeField] private string fightID;
    [SerializeField] private List<EnemySpawner> enemySpawners;

    private readonly Dictionary<Enemy, int> spawnerIndexByEnemy = new Dictionary<Enemy, int>();
    private FightState fightState;
    private bool isFightComplete;

    private void Start()
    {
        fightState = SaveLoadService.Instance.Get<GeneralSaveFile>().FightsSnapshot.GetFight(fightID);

        if (fightState != null && RemainingEnemyCount() == 0)
        {
            CompleteFight();
        }
        else
        {
            StartFight();
        }
    }

    public void StartFight()
    {
        if (isFightComplete || spawnerIndexByEnemy.Count > 0)
            return;

        if (fightState == null)
        {
            fightState = new FightState(fightID, FullEnemyCounts());
            SaveLoadService.Instance.Get<GeneralSaveFile>().FightsSnapshot.Fights.Add(fightState);
        }

        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < enemySpawners.Count; i++)
        {
            List<Enemy> spawned = enemySpawners[i].SpawnEnemies(fightState.RemainingEnemies[i]);

            foreach (Enemy enemy in spawned)
            {
                spawnerIndexByEnemy[enemy] = i;
                enemy.OnDeath += OnEnemyDeath;
            }

            enemies.AddRange(spawned);
        }

        FightManager.Instance.SetUpFight(this, enemies);

        SaveLoadService.Instance.SaveProgress();
    }

    public void CompleteFight()
    {
        isFightComplete = true;
    }

    public bool IsFightComplete()
    {
        return isFightComplete;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyDeath;

        fightState.RemainingEnemies[spawnerIndexByEnemy[enemy]]--;
        spawnerIndexByEnemy.Remove(enemy);

        SaveLoadService.Instance.SaveProgress();
    }

    private int RemainingEnemyCount()
    {
        int count = 0;

        foreach (int remaining in fightState.RemainingEnemies)
            count += remaining;

        return count;
    }

    private List<int> FullEnemyCounts()
    {
        List<int> counts = new List<int>();

        foreach (EnemySpawner spawner in enemySpawners)
            counts.Add(spawner.EnemyCount);

        return counts;
    }
}
