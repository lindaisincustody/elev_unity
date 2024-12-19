using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisuals : Component
{
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer shader;

    [Header("Shield")]
    [SerializeField] private Material shieldMat;
    [SerializeField] private string valueToChange;
    [SerializeField] private float activeValue;
    [SerializeField] private float inactiveValue;

     private float duration = 0.5f;

    private Tween shieldTween;

    private Material shieldMatInstance;

    private void Awake()
    {
        shieldMatInstance = new Material(shieldMat);
        shader.material = shieldMatInstance;
    }

    private void Start()
    {
        ActivateShield();
    }

    private void LateUpdate()
    {
        shader.sprite = body.sprite;
    }

    public void ActivateShield()
    {
        shieldTween?.Kill();
        shieldTween = shieldMatInstance.DOFloat(activeValue, valueToChange, duration);
    }

    public void DeactivateShield()
    {
        shieldTween?.Kill();
        shieldTween = shieldMatInstance.DOFloat(inactiveValue, valueToChange, duration);
    }
}
