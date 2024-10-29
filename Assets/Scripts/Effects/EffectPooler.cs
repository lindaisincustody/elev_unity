using System.Collections.Generic;
using UnityEngine;

public class EffectPooler
{
    private Dictionary<EffectType, Queue<GameObject>> effectPool = new();

    public EffectPooler()
    {
        foreach (EffectType effectType in System.Enum.GetValues(typeof(EffectType)))
        {
            effectPool[effectType] = new Queue<GameObject>();
        }
    }

    public GameObject GetPooledEffect(EffectType type)
    {
        if (effectPool[type].Count > 0)
        {
            GameObject pooledEffect = effectPool[type].Dequeue();
            pooledEffect.SetActive(true);
            return pooledEffect;
        }

        return null;
    }

    public void ReturnEffectToPool(EffectType type, GameObject effect)
    {
        effect.SetActive(false);
        effectPool[type].Enqueue(effect);
    }
}

public enum EffectType
{
    Stun
}