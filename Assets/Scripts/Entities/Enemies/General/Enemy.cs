using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        SanityBar.instance.OnSanityChange += SanityChange;
        SanityChange(0);
        SetBounds();
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
