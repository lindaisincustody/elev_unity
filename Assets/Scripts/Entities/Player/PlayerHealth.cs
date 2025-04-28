using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class PlayerHealth : Health
{
    private SpriteRenderer spriteRenderer;
    private Tween flashTween;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        spriteRenderer = Player.instance.spriteRenderer;
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
        Flash(flashType);
    }

    private void Flash(FlashType flashType)
    {
        flashTween?.Kill();
        spriteRenderer.color = Color.white;
        Color color = flashType == FlashType.Damage ? Color.red : Color.green;
        flashTween = spriteRenderer.DOColor(color, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}
