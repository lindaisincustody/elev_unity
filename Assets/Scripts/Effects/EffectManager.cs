using System.Collections.Generic;
using UnityEngine;

public class EffectManager
{
    private Dictionary<EffectType, GameObject> effectDictionary = new();

    public void LoadEffects()
    {
        foreach (EffectType effectType in System.Enum.GetValues(typeof(EffectType)))
        {
            string path = $"Effects/{effectType}Effect";
            GameObject loadedEffect = LoadFromResources(path);

            if (loadedEffect != null)
            {
                effectDictionary[effectType] = loadedEffect;
            }
        }
    }

    public GameObject GetEffectPrefab(EffectType type)
    {
        if (effectDictionary.TryGetValue(type, out GameObject effectObj))
        {
            return effectObj;
        }

        return null;
    }

    private GameObject LoadFromResources(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path); // Loads the prefab from Resources folder
        if (prefab != null)
        {
            Debug.Log("Successfully loaded and retrieved the StunEffect prefab.");
        }
        else
        {
            Debug.Log("Failed to load the StunEffect prefab.");
        }
        return prefab;
    }
}
