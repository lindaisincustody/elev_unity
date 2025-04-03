using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawnFireTrigger : MonoBehaviour
{
    private int damage = 2;
    private float damageInterval = 0.5f;

    private Dictionary<Entity, DamageOvertimeEffect> activeEffects = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Entity entity = collision.GetComponent<Entity>();
            DamageOvertimeEffect effect = new DamageOvertimeEffect(entity, damage, damageInterval);
            entity.Get<EntityEffects>().Add(effect);
            activeEffects.Add(entity, effect);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Entity entity = collision.GetComponent<Entity>();
            if (activeEffects.TryGetValue(entity, out DamageOvertimeEffect effect))
            {
                entity.Get<EntityEffects>().Remove(effect);
                activeEffects.Remove(entity);
            }
        }
    }
}
