using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCollider : MonoBehaviour
{
    [SerializeField] Gun gun;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Consumable"))
        {
            if (collision.GetComponentInParent<SoftBody>() != null)
                gun.SuckInSoftBody(collision.GetComponentInParent<SoftBody>());
        }

    }
}
