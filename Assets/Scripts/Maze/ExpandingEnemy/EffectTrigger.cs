using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    private EffectManager effectManager;
    private ExpandingEnemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        effectManager = FindObjectOfType<EffectManager>();
        if (GetComponent<ExpandingEnemy>() != null)
            enemy = GetComponent<ExpandingEnemy>();
        else
            enemy = GetComponentInParent<ExpandingEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (effectManager.ActivateCircleMaskEffect())
            {
                enemy.PlayerHit(collision.gameObject);
            }
        }
    }
}
