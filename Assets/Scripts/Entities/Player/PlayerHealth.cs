using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    void Start()
    {
        currentHealth = maxHealth;
    }

    protected override void Die()
    {
        // TODO: implement respawn
        transform.gameObject.SetActive(false);
        Debug.Log("You died");
    }

    protected override void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;

        Debug.Log("You took damage. HP: " + healthPercentage);
        // TODO: make a UI for player health
    }

}
