using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightInteractable : MonoBehaviour
{
    [SerializeField] private Fight fight;

    public System.Action OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetupFight();
        }
    }

    public void SetupFight()
    {
        fight.StartFight();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
        }
    }
}
