using System.Collections.Generic;
using UnityEngine;

public class Fight : MonoBehaviour
{
    [SerializeField] private string fightID;
    [SerializeField] private List<EnemySpawner> enemySpawners;
    [Header("Fight Win Unlock")]
    [SerializeField] private Teleporter teleporter;

    private List<Enemy> enemies = new List<Enemy>();
    private bool isFightComplete;

    private string defaultFightId = "Reception_Fight";

    private void Start()
    {
        PlayerData playerData = SavingWrapper.Instance.LoadPlayerData();
        string lastFightId;
        if (string.IsNullOrEmpty(playerData.lastFightID))
        {
            lastFightId = defaultFightId;
        }
        else
        {
            lastFightId = playerData.lastFightID;
        }

        if (lastFightId == fightID)
        {
            StartFight();
        }
    }

    public void StartFight()
    {
        if (isFightComplete)
        {
            isFightComplete = false;

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null)
                {
                    Destroy(enemies[i].gameObject);
                }
            }
            enemies.Clear();

            if (teleporter != null)
            {
                teleporter.Unlocked = false;
                teleporter.gameObject.SetActive(false);
            }
        }

        enemies.Clear();
        foreach (var spawner in enemySpawners)
        {
            List<Enemy> spawned = spawner.SpawnEnemies();
            enemies.AddRange(spawned);
        }

        FightManager.Instance.SetUpFight(this, enemies);

        Player.instance.playerData.lastFightID = fightID;
    }

    public void CompleteFight()
    {
        if (teleporter != null)
        {
            teleporter.Unlocked = true;
            teleporter.gameObject.SetActive(true);
        }
        isFightComplete = true;
    }

    public bool IsFightComplete()
    {
        return isFightComplete;
    }
}
