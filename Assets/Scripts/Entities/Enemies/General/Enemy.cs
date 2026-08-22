using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private GameObject underworldBody;
    [SerializeField] private GameObject overworldBody;
    [SerializeField] private EnemyLetter letterPrefab;

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

        List<string> filteredLabels = new List<string>(_labels);

        float symbolSpacing = 1f;
        int symbolCount = filteredLabels.Count;
        float totalWidth = (symbolCount - 1) * symbolSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < symbolCount; i++)
        {
            Vector3 symbolPosition = transform.position + new Vector3(startX + i * symbolSpacing, 4, 0);
            EnemyLetter symbolObject = Instantiate(letterPrefab, symbolPosition, Quaternion.identity, transform);
            enemyLetters.Add(symbolObject);

            EnemyGlyph randomSymbol = new(Glyphs.LatexToUnicode.ContainsKey(filteredLabels[i]) ? Glyphs.LatexToUnicode[filteredLabels[i]] : filteredLabels[i]);
            if (UnityEngine.Random.value <= Player.instance.SpecialSymbolChance)
            {
                SetSymbolSpecial(randomSymbol, symbolObject.Letter);
            }

            symbolObject.Letter.text = randomSymbol.Glyph;
            displayedSymbols.Add(symbolObject.Letter);
            activeSymbols.Add(randomSymbol);
        }

        Get<EnemyHealth>().Immune = true;
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
                    UIManager.Instance.Get<GlyphBook>().GlyphWritten(this, matchedSymbol);
                    Destroy(matchedSymbol.gameObject);   
                }

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
                    Get<EnemyHealth>().Immune = false;
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
