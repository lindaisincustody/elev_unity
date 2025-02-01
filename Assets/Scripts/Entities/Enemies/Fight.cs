using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fight : MonoBehaviour
{
    [SerializeField] private List<SpawnEnemy> enemySpawners;
    [SerializeField] private Item reward;

    private List<Enemy> enemies = new();

    // With more rooms, have this as method instead of Awake
    // Then when entering a room, activat this (have a bool)
    private void Awake()
    {
        foreach (SpawnEnemy spawner in enemySpawners)
        {
            spawner.OnSpawn += RegisterEnemy;
        }
    }

    private void Start()
    {
        SanityBar.instance.sanityEffectHandler.OnWorldChange += SetUpGlyphs;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            foreach (Enemy enemy in enemies)
            {
                enemy.Get<EnemyHealth>().TakeDamage(999999);
            }
    }

    public void SetUpGlyphs()
    {
        if (!SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
            return;

        GlyphBook.instance.ActivateBook();
        foreach (Enemy enemy in enemies)
        {
            GlyphBook.instance.AddEnemy(enemy);
        }
    }

    private void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
        enemy.OnDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyDeath;
        enemies.Remove(enemy);

        if (enemies.Count == 0)
            CompleteFight();
    }

    private void CompleteFight()
    {
        GlyphBook.instance.TranslateGlyphs();
        SanityBar.instance.SanityToMax();
        Player.instance.ItemsInventory.AddItem(reward);
    }

    private void OnDestroy()
    {
        foreach (SpawnEnemy spawner in enemySpawners)
        {
            spawner.OnSpawn -= RegisterEnemy;
        }
    }
}
