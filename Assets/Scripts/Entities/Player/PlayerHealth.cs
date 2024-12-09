using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    protected override void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;
        OnDamage?.Invoke();
        Flash();
    }

    private void Flash()
    {
        flashTween?.Kill();
        spriteRenderer.color = Color.white;
        flashTween = spriteRenderer.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}
