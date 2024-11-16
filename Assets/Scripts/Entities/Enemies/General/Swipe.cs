using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    [SerializeField] private Collider2D collider2D;

    private Health targetHeatlh;
    private int damage;

    private WaitForSeconds wait;

    private void Awake()
    {
        wait = new WaitForSeconds(0.1f);
    }

    public void Init(Health target, int damage)
    {
        targetHeatlh = target;
        this.damage = damage;
    }

    public void ActivateCollider()
    {
        collider2D.enabled = true;
        StartCoroutine(DeactivateCollider());
    }

    public IEnumerator DeactivateCollider()
    {
        yield return wait;
        collider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetHeatlh.TakeDamage(damage);
        }
    }
}
