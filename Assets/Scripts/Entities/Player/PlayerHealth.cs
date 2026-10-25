using UnityEngine;

public class PlayerHealth : Health
{
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    protected override void Die()
    {
        // TODO: implement respawn
        transform.gameObject.SetActive(false);
        Debug.Log("You died");
    }

    protected override void UpdateHealthBar(FlashType flashType)
    {
        float healthPercentage = currentHealth / maxHealth;
        OnDamage?.Invoke();
        Entity.Get<PlayerVisuals>().Flash(flashType == FlashType.Damage ? Color.red : Color.green);
    }
}
