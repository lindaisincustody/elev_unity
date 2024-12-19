using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject underworldBody;
    [SerializeField] private GameObject overworldBody;

    [SerializeField] private List<Component> components;

    private Dictionary<System.Type, Component> componentCache = new Dictionary<System.Type, Component>();

    public Vector2 minBound { get; set; }
    public Vector2 maxBound { get; set; }

    [SerializeField] private GameObject symbolTextEnemy;
    private List<TMP_Text> displayedSymbols = new List<TMP_Text>();
    public HashSet<string> activeSymbols = new HashSet<string>();
    private EnemyHealth enemyHealth;


    private readonly string[] _labels = {
        "_Capricorn",
        "_Heart",
        "_Leo",
        "_Moon",
        "_Rightarrow",
        "_bowtie",
        "_clubsuit",
        "_descnode",
        "_diagup",
        "_diamond",
        "_downarrow",
        "_infty",
        "_ocircle",
        "_oplus",
        "_spadesuit",
        "_square",
        "_star",
        "_textgamma",
        "_textmusicalnote",
        "_varphi"
    };

    private readonly Dictionary<string, string> latexToUnicode = new Dictionary<string, string>
{
    { "_Capricorn", "♑" },
    { "_Heart", "♥" },
    { "_Leo", "♌" },
    { "_Moon", "☾" },
    { "_Rightarrow", "⇒" },
    { "_bowtie", "⧓" },
    { "_clubsuit", "♣" },
    { "_descnode", "⤵" },
    { "_diagup", "/" },
    { "_diamond", "♦" },
    { "_downarrow", "↓" },
    { "_infty", "∞" },
    { "_ocircle", "⦾" },
    { "_oplus", "⊕" },
    { "_spadesuit", "♠" },
    { "_square", "■" },
    { "_star", "★" },
    { "_textgamma", "γ" },
    { "_textmusicalnote", "♪" },
    { "_varphi", "φ" }
};

    private void Awake()
    {
        foreach (var component in components)
        {
            component.Init(this);
        }
    }

    void Start()
    {
        SanityBar.instance.OnSanityChange += SanityChange;
        SanityChange(0);
        SetBounds();
        GenerateRandomSymbols();

        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void GenerateRandomSymbols()
    {
        float symbolSpacing = 1f;

        // Create a filtered list of labels excluding specific symbols
        var filteredLabels = new List<string>(_labels);
        filteredLabels.Remove("_descnode");
        filteredLabels.Remove("_textmusicalnote");
        filteredLabels.Remove("_oplus");
        filteredLabels.Remove("_infty");

        for (int i = 0; i < 3; i++)
        {
            // Create a symbol object above the enemy with horizontal alignment
            Vector3 symbolPosition = transform.position + new Vector3(i * symbolSpacing - 1, 4, 0); // Increment x-axis for horizontal layout
            GameObject symbolObject = Instantiate(symbolTextEnemy, symbolPosition, Quaternion.identity, transform);

            TMP_Text symbolText = symbolObject.GetComponent<TMP_Text>();

            // Assign a random symbol from the filtered list
            string randomSymbolKey = filteredLabels[UnityEngine.Random.Range(0, filteredLabels.Count)];
            string randomSymbol = latexToUnicode.ContainsKey(randomSymbolKey) ? latexToUnicode[randomSymbolKey] : randomSymbolKey;
            symbolText.text = randomSymbol;
            displayedSymbols.Add(symbolText);
            activeSymbols.Add(randomSymbol);
        }
    }


    public void CheckSymbolMatch(string drawnSymbol)
    {
        if (activeSymbols.Contains(drawnSymbol))
        {
            // Find all matching symbols in displayedSymbols
            var matchedSymbols = displayedSymbols.FindAll(symbol => symbol.text == drawnSymbol);

            if (matchedSymbols.Count > 0)
            {
                Debug.Log($"Destroying all instances of symbol: {drawnSymbol}");
                foreach (var matchedSymbol in matchedSymbols)
                {
                    displayedSymbols.Remove(matchedSymbol); // Remove from list
                    GlyphBook.instance.GlyphWritten(this, matchedSymbol);
                    Destroy(matchedSymbol.gameObject);      // Destroy the GameObject
                }

                // Remove the symbol from activeSymbols
                enemyHealth.StartCoroutine(enemyHealth.FlashWhite());
                activeSymbols.Remove(drawnSymbol);

                // Check if all symbols have been destroyed
                if (activeSymbols.Count == 0)
                {
                    Debug.Log("All symbols destroyed. Stun enemy.");
                    // Trigger the stun effect
                    Get<Stun>().Execute();
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

    private void SanityChange(int amount)
    {
        if (SanityEffectHandler.IsPlayerInUnderworld)
            ShowEnemy();
        else
            ShowSparkle();
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

    void OnDestroy()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy(this);
        }
    }
}
