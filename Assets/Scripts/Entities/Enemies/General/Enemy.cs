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

            // Assign a random letter
            char randomLetter = (char)('A' + UnityEngine.Random.Range(0, 26));
            letterText.text = randomLetter.ToString();
            displayedLetters.Add(letterText);
            activeLetters.Add(randomLetter);
        }
    }

    public void CheckLetterMatch(char drawnLetter)
    {
        if (activeLetters.Contains(drawnLetter))
        {
            // Find and remove the matched letter
            TMP_Text matchedLetter = displayedLetters.Find(letter => letter.text == drawnLetter.ToString());
            if (matchedLetter != null)
            {
                displayedLetters.Remove(matchedLetter);
                activeLetters.Remove(drawnLetter);
                Destroy(matchedLetter.gameObject);
            }

            // If no letters remain, call Die() in EnemyHealth
            if (activeLetters.Count == 0 && enemyHealth != null)
            {
                enemyHealth.StartCoroutine(enemyHealth.Die());
            }
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
