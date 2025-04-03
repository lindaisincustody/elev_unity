using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image mask;
    [SerializeField] private TextMeshProUGUI healthText;

    private Player player;
    private PlayerHealth playerHealth;

    // TODO: Health Bar should be used for both enemy and player
    private void Start()
    {
        player = Player.instance;
        playerHealth = player.Get<PlayerHealth>();
        playerHealth.OnDamage += UpdateHealthBar;

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float currentOffset = playerHealth.currentHealth;
        float maximumOffset = playerHealth.maxHealth;
        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;
        healthText.text = currentOffset + "/" + maximumOffset;
    }

    private void OnDestroy()
    {
        playerHealth.OnDamage -= UpdateHealthBar;
    }
}
