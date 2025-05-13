using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityEffect
{
    public Entity entity;

    public EntityEffect(Entity entity)
    {
        this.entity = entity;
    }

    public abstract void Dispose();
}

