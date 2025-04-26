using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawnFireTrigger : MonoBehaviour
{
    public LayerMask LayerMask { get; set; }
    public float duration { get; set; } = 5f;
    public int damage { get; set; } = 1;
    public float damageInterval { get; set; } = 0.5f;

    private Dictionary<Entity, DamageOvertimeEffect> activeEffects = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & LayerMask.value) != 0 && collision.isTrigger)
        {
            Entity entity = collision.GetComponent<Entity>();
            DamageOvertimeEffect effect = new DamageOvertimeEffect(entity, damage, damageInterval);
            entity.Get<EntityEffects>().Add(effect);
            activeEffects.Add(entity, effect);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & LayerMask.value) != 0 && collision.isTrigger)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (activeEffects.TryGetValue(entity, out DamageOvertimeEffect effect))
            {
                entity.Get<EntityEffects>().Remove(effect);
                activeEffects.Remove(entity);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(LifeCycle());
    }

    private IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }    
}
