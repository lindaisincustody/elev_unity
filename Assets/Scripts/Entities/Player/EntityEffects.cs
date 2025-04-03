using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffects : Component
{
    public List<EntityEffect> effects { get; private set; } = new();

    public void Add(EntityEffect effect)
    {
        effects.Add(effect);
    }

    public void Remove(EntityEffect effect)
    {
        if (effects.Contains(effect))
        {
            effect.Dispose();
            effects.Remove(effect);
        }
    }
}
