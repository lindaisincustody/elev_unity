using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance { get; private set; }

    private Fight _activeFight;
    private List<Enemy> enemies = new();

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

    public void SetUpFight(Fight fight, List<Enemy> newEnemeies)
    {
        _activeFight = fight;
        enemies = newEnemeies;
        foreach (Enemy enemy in enemies.ToArray())
        {
            RegisterEnemy(enemy);
        }

        SetUpGlyphs();
    }

    private void RegisterEnemy(Enemy enemy)
    {
        enemy.OnDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyDeath;
        enemies.Remove(enemy);
        if (enemies.Count == 0)
            CompleteFight();
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

    private void CompleteFight()
    {
        GlyphBook.instance.TranslateGlyphs();
        SanityBar.instance.SanityToMax();
        _activeFight.CompleteFight();
    }

    private void OnDestroy()
    {
        SanityBar.instance.sanityEffectHandler.OnWorldChange -= SetUpGlyphs;
    }
}
