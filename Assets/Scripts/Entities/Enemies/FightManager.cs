using System.Collections.Generic;
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
        // Example: hooking up a "world change" event
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
    }


    /// <summary>
    /// Registers a new fight. Kills off previous fight’s enemies if there was an active fight.
    /// </summary>
    public void SetUpFight(Fight newFight, List<Enemy> newEnemies)
    {
        // If there's an existing fight, kill all existing enemies
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

        // Set the new fight as active
        _activeFight = newFight;
        _activeEnemies = newEnemies;

        // Register each enemy death callback
        foreach (Enemy enemy in _activeEnemies)
        {
            enemy.OnDeath += OnEnemyDeath;
        }

        // Update glyphs if needed
        SetUpGlyphs();
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyDeath;
        _activeEnemies.Remove(enemy);

        // If no enemies remain, the fight is over
        if (_activeEnemies.Count == 0)
        {
            CompleteFight();
        }
    }

    /// <summary>
    /// Called once the current fight’s enemies have all died.
    /// </summary>
    private void CompleteFight()
    {
        GlyphBook.instance.TranslateGlyphs();
        SanityBar.instance.SanityToMax();

        // Let the Fight script do any final unlocking or effects
        _activeFight.CompleteFight();
    }

    /// <summary>
    /// Called when the underworld is toggled; if in underworld, show glyphs for active enemies.
    /// </summary>
    public void SetUpGlyphs()
    {
        if (!SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld) return;

        GlyphBook.instance.ActivateBook();
        foreach (Enemy enemy in _activeEnemies)
        {
            GlyphBook.instance.AddEnemy(enemy);
        }
    }

    private void OnDestroy()
    {
        // Example unsubscribing from the "world change" event
        SanityBar.instance.sanityEffectHandler.OnWorldChange -= SetUpGlyphs;
    }
}
