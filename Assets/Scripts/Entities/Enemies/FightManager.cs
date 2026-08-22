using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance { get; private set; }

    private Fight _activeFight;
    private List<Enemy> _activeEnemies = new List<Enemy>();

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
        {
            foreach (Enemy enemy in _activeEnemies)
            {
                enemy.Get<EnemyHealth>().TakeDamage(999999);
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (Enemy enemy in _activeEnemies)
            {
                foreach (EnemyGlyph symbol in enemy.activeSymbols.ToList())
                {
                    enemy.CheckSymbolMatch(symbol.Glyph);
                }
            }
        }
    }


    public void SetUpFight(Fight newFight, List<Enemy> newEnemies)
    {
        if (_activeFight != null && _activeFight != newFight)
        {
            foreach (Enemy oldEnemy in _activeEnemies)
            {
                if (oldEnemy != null && oldEnemy.Get<EnemyHealth>().IsAlive)
                {
                    oldEnemy.Get<EnemyHealth>().TakeDamage(9999999);
                }
            }
        }

        _activeFight = newFight;
        _activeEnemies = newEnemies;

        foreach (Enemy enemy in _activeEnemies)
        {
            enemy.OnDeath += OnEnemyDeath;
        }

        SetUpGlyphs();
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyDeath;
        _activeEnemies.Remove(enemy);

        if (_activeEnemies.Count == 0)
        {
            CompleteFight();
        }
    }

    private void CompleteFight()
    {
        UIManager.Instance.Get<GlyphBook>().TranslateGlyphs();
        SanityBar.instance.SanityToMax();

        _activeFight.CompleteFight();
    }

    public void SetUpGlyphs()
    {
        if (!SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld) return;

        UIManager.Instance.Get<GlyphBook>().ActivateBook();
        foreach (Enemy enemy in _activeEnemies)
        {
            UIManager.Instance.Get<GlyphBook>().AddEnemy(enemy);
        }
    }

    private void OnDestroy()
    {
        SanityBar.instance.sanityEffectHandler.OnWorldChange -= SetUpGlyphs;
    }
}
