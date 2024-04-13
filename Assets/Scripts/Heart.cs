using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public TMP_Text hpText;
    public int hp = 3; // Initial health points

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile")) // Make sure your projectile has this tag
        {
            hp -= 1; // Decrease health by one
           // Destroy(collision.gameObject); // Destroy the projectile

            if (hp <= 0)
            {
                // Optionally handle the heart's destruction or game over logic here
                Debug.Log("Heart destroyed!");
            }
        }
    }

    private void Update()
    {
        UpdateHPText();
        Debug.Log(hp);
    }
    void UpdateHPText()
    {
        // Update the TMP text to display the current value of wonLevels
        hpText.text = hp.ToString();
    }
}