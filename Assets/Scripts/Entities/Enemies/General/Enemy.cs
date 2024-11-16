using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject underworldBody;
    [SerializeField] private GameObject overworldBody;

    [SerializeField] private List<Component> components;

    private Dictionary<System.Type, Component> componentCache = new Dictionary<System.Type, Component>();

    public Vector2 minBound { get; set; }
    public Vector2 maxBound { get; set; }

    [SerializeField] private GameObject letterTextEnemy;
    private List<TMP_Text> displayedLetters = new List<TMP_Text>();
    private HashSet<char> activeLetters = new HashSet<char>();
    private EnemyHealth enemyHealth;

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
        GenerateRandomLetters();

        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void GenerateRandomLetters()
    {
        float letterSpacing = 0.5f;
        for (int i = 0; i < 3; i++)
        {
            // Create a letter object above the enemy with horizontal alignment
            Vector3 letterPosition = transform.position + new Vector3(i * letterSpacing, 1, 0); // Increment x-axis for horizontal layout
            GameObject letterObject = Instantiate(letterTextEnemy, letterPosition, Quaternion.identity, transform);

            TMP_Text letterText = letterObject.GetComponent<TMP_Text>();

            char randomLetter;

            // Assign a random letter, excluding 'L'
            do
            {
                randomLetter = (char)('A' + UnityEngine.Random.Range(0, 26));
            }
            while (randomLetter == 'L'); // Keep re-generating until it's not 'L'

            letterText.text = randomLetter.ToString();
            displayedLetters.Add(letterText);
            activeLetters.Add(randomLetter);
        }
    }

    public void CheckLetterMatch(char drawnLetter)
    {
        char upperCaseLetter = char.ToUpper(drawnLetter);

        if (activeLetters.Contains(upperCaseLetter))
        {
            // Find all matching letters in displayedLetters
            var matchedLetters = displayedLetters.FindAll(letter => letter.text.Equals(upperCaseLetter.ToString(), StringComparison.OrdinalIgnoreCase));

            if (matchedLetters.Count > 0)
            {
                Debug.Log($"Destroying all instances of letter: {upperCaseLetter}");
                foreach (var matchedLetter in matchedLetters)
                {
                    displayedLetters.Remove(matchedLetter); // Remove from list
                    Destroy(matchedLetter.gameObject);      // Destroy the GameObject
                }

                // Remove the letter from activeLetters
                enemyHealth.StartCoroutine(enemyHealth.FlashWhite());
                activeLetters.Remove(upperCaseLetter);
                

                // Check if all letters have been destroyed
                if (activeLetters.Count == 0 && enemyHealth != null)
                {
                    Debug.Log("All letters destroyed. Enemy dying.");
                    enemyHealth.StartCoroutine(enemyHealth.Die());
                }
            }
            else
            {
                Debug.LogError($"No matching visual letter found for: {upperCaseLetter}, but it exists in activeLetters.");
            }
        }
        else
        {
            Debug.Log($"Letter {drawnLetter} does not match any active letters.");
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
