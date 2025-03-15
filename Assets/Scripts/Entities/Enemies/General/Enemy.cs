using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject underworldBody;
    [SerializeField] private GameObject overworldBody;
    [SerializeField] private EnemyLetter letterPrefab;

    [SerializeField] private List<Component> components;

    private Dictionary<System.Type, Component> componentCache = new Dictionary<System.Type, Component>();

    public HashSet<EnemyGlyph> activeSymbols = new HashSet<EnemyGlyph>();

    public Vector2 minBound { get; set; }
    public Vector2 maxBound { get; set; }

    public Action<Enemy> OnDeath { get; set; }

    private List<TMP_Text> displayedSymbols = new List<TMP_Text>();

    private List<EnemyLetter> enemyLetters = new();

    private void Awake()
    {
        foreach (var component in components)
        {
            component.Init(this);
        }

        SanityBar.instance.sanityEffectHandler.OnWorldChange += SanityChange;
    }

    void Start()
    {
        SanityChange();
        SetBounds();
        HideGlyphs();

        Get<EnemyHealth>().OnDeath += OnEnemyDeath;
    }

    public void GenerateRandomSymbols(string[] _labels)
    {
        if (activeSymbols.Count > 0)
            return;

        float symbolSpacing = 1f;

        var filteredLabels = new List<string>(_labels);

        for (int i = 0; i < 3; i++)
        {
            Vector3 symbolPosition = transform.position + new Vector3(i * symbolSpacing - 1, 4, 0);
            EnemyLetter symbolObject = Instantiate(letterPrefab, symbolPosition, Quaternion.identity, transform);
            enemyLetters.Add(symbolObject);
            string randomSymbolKey = filteredLabels[UnityEngine.Random.Range(0, filteredLabels.Count)];
            EnemyGlyph randomSymbol = new(Glyphs.LatexToUnicode.ContainsKey(randomSymbolKey) ? Glyphs.LatexToUnicode[randomSymbolKey] : randomSymbolKey);
            if (UnityEngine.Random.value <= Player.instance.SpecialSymbolChance)
            {
                SetSymbolSpecial(randomSymbol, symbolObject.Letter);
            }

            symbolObject.Letter.text = randomSymbol.Glyph;
            displayedSymbols.Add(symbolObject.Letter);
            activeSymbols.Add(randomSymbol);
        }
    }

    private void SetSymbolSpecial(EnemyGlyph glyph, TMP_Text glyphText)
    {
        glyph.SetSpecial(() => Get<Stun>().Execute());
        glyphText.color = Color.white;
    }


    public void CheckSymbolMatch(string drawnSymbol)
    {
        if (activeSymbols.Any(g => g.Glyph == drawnSymbol))
        {
            var matchedSymbols = displayedSymbols.FindAll(symbol => symbol.text == drawnSymbol);

            if (matchedSymbols.Count > 0)
            {
                Debug.Log($"Destroying all instances of symbol: {drawnSymbol}");
                foreach (var matchedSymbol in matchedSymbols)
                {
                    displayedSymbols.Remove(matchedSymbol); 
                    GlyphBook.instance.GlyphWritten(this, matchedSymbol);
                    Destroy(matchedSymbol.gameObject);   
                }

                Get<EnemyHealth>().StartCoroutine(Get<EnemyHealth>().FlashWhite());

                List<EnemyGlyph> matchedGlyphs = new List<EnemyGlyph>();

                foreach (var glyph in activeSymbols)
                {
                    if (glyph.Glyph == drawnSymbol)
                    {
                        glyph.OnDrawn();
                        matchedGlyphs.Add(glyph);
                    }
                }

                foreach (var glyph in matchedGlyphs)
                {
                    activeSymbols.Remove(glyph);
                }

                if (activeSymbols.Count == 0)
                {
                    Debug.Log("All symbols destroyed.");
                    Get<EnemyVisuals>().DeactivateShield();
                    Get<EnemyHealth>().ActivateHealthBar();
                }
            }
            else
            {
                Debug.LogError($"No matching visual symbol found for: {drawnSymbol}, but it exists in activeSymbols.");
            }
        }
        else
        {
            Debug.Log($"Symbol {drawnSymbol} does not match any active symbols.");
        }
    }


    public T Get<T>() where T : Component
    {
        var type = typeof(T);

        if (componentCache.TryGetValue(type, out Component cachedComponent))
        {
            return cachedComponent as T;
        }

        foreach (var item in components)
        {
            if (item is T)
            {
                componentCache[type] = item;
                return item as T;
            }
        }

        return null;
    }

    private void SetBounds()
    {
        Get<EnemyMovement>().minBound = minBound;
        Get<EnemyMovement>().maxBound = maxBound;
    }

    private void SanityChange()
    {
        if (SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
        {
            ShowGlyphs();
            ShowEnemy();
        }
        else
        {
            HideGlyphs();
            ShowSparkle();
        }
    }

    private void ShowEnemy()
    {
        underworldBody.SetActive(true);
        overworldBody.SetActive(false);
    }

    private void ShowSparkle()
    {
        underworldBody.SetActive(false);
        overworldBody.SetActive(true);
    }

    private void ShowGlyphs()
    {
        foreach (EnemyLetter letter in enemyLetters)
        {
            letter.Show();
        }
    }

    private void HideGlyphs()
    {
        foreach (EnemyLetter letter in enemyLetters)
        {
            letter.Hide();
        }
    }

    private void OnEnemyDeath()
    {
        OnDeath?.Invoke(this);
    }

    void OnDestroy()
    {
        SanityBar.instance.sanityEffectHandler.OnWorldChange -= SanityChange;
        Get<EnemyHealth>().OnDeath -= OnEnemyDeath;
    }
}
