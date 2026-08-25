using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyManager : CoreService
{
    public static EnemyManager Instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public List<Enemy> GetAllEnemies()
    {
        return enemies;
    }
}
