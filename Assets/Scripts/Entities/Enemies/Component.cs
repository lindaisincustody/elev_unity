using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Component : MonoBehaviour
{
    protected Entity Entity;

    public void Init(Entity entity)
    {
        Entity = entity;
    }
}
