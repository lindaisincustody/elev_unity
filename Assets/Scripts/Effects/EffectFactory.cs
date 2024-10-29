using UnityEngine;

public class EffectFactory
{
    private EffectManager effectManager;

    public EffectFactory(EffectManager manager)
    {
        effectManager = manager;
    }

    public GameObject CreateEffect(EffectType type)
    {
        GameObject effectPrefab = effectManager.GetEffectPrefab(type);
        if (effectPrefab != null)
        {
            return InstantiateEffect(effectPrefab);
        }

        return null;
    }

    private GameObject InstantiateEffect(GameObject prefab)
    {
        return Object.Instantiate(prefab);
    }
}
