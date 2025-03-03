using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightInteractable : MonoBehaviour
{
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
        OnTriggerEnter?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
        }
    }
}
