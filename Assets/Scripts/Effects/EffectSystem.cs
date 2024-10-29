using UnityEngine;

public static class EffectSystem
{
    private static EffectPooler pooler;
    private static EffectFactory factory;

    static EffectSystem()
    {
        EffectManager effectManager = new EffectManager();
        effectManager.LoadEffects();

        pooler = new EffectPooler();
        factory = new EffectFactory(effectManager);
    }

    public static GameObject GetEffect(EffectType type)
    {
        GameObject effect = pooler.GetPooledEffect(type);
        if (effect == null)
        {
            effect = factory.CreateEffect(type);
        }

        return effect;
    }

    public static void ReturnEffect(EffectType type, GameObject effect)
    {
        pooler.ReturnEffectToPool(type, effect);
    }
}
