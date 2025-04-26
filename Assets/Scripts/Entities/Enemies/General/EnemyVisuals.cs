using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
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
    [SerializeField] private float flashDuration = 1f;

    private float duration = 0.5f;

    private Tween shieldTween;
    private Tween flashTween;

    private Material shieldMatInstance;
    private Material bodyMatInstance;

    private DissolveEffect dissolveEffect;

    private void Awake()
    {
        shieldMatInstance = new Material(shieldMat);
        bodyMatInstance = new Material(body.material);

        shader.material = shieldMatInstance;
        body.material = bodyMatInstance;

        dissolveEffect = new DissolveEffect(bodyMatInstance, Entity.Get<EnemyHealth>().DeathDuration);

        Entity.Get<EnemyHealth>().OnDamage += Flash;
        Entity.Get<EnemyHealth>().OnLethal += Vanish;
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

    public void Vanish()
    {
        dissolveEffect.Vanish();
    }

    private void Flash()
    {
        flashTween?.Kill();

        bodyMatInstance.SetFloat("_FlashIntensity", 4f);

        flashTween = bodyMatInstance
        .DOFloat(0f, "_FlashIntensity", 0.2f)
        .SetEase(Ease.Linear);
    }

    public void Appear()
    {
        dissolveEffect.Appear();
    }

    private void OnDestroy()
    {
        Entity.Get<EnemyHealth>().OnLethal -= Vanish;
        Entity.Get<EnemyHealth>().OnDamage -= Flash;
    }
}
