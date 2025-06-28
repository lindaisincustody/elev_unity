using System.Collections;
using System.Linq;
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

    public static IEnumerator ReturnEffectOnComplete(EffectType type, GameObject effect)
    {
        var systems = effect.GetComponentsInChildren<ParticleSystem>();
        if (systems.Length > 0)
        {
            yield return new WaitUntil(() =>
                systems.All(ps => !ps.IsAlive(true))
            );
        }
        else
        {
            yield return null;
        }

        effect.transform.localScale = new Vector3(
            Mathf.Abs(effect.transform.localScale.x),
            Mathf.Abs(effect.transform.localScale.y),
            Mathf.Abs(effect.transform.localScale.z)
        );

        // 3) Return to pool
        pooler.ReturnEffectToPool(type, effect);
    }

    public static void ReturnEffect(EffectType type, GameObject effect)
    {
        Vector3 positiveScale = new Vector3(
               Mathf.Abs(effect.transform.localScale.x),
               Mathf.Abs(effect.transform.localScale.y),
               Mathf.Abs(effect.transform.localScale.z)
           );
        effect.transform.localScale = positiveScale;
        pooler.ReturnEffectToPool(type, effect);
    }
}
