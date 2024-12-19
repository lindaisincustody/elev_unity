using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUpGlyphs()
    {
        GlyphBook.instance.ActivateBook();
        foreach (Enemy enemy in enemies)
        {
            GlyphBook.instance.AddEnemy(enemy);
        }
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
