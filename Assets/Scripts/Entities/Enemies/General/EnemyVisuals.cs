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

    private float duration = 0.5f;
    private Tween shieldTween;
    private Material shieldMatInstance;
    private DissolveEffect dissolveEffect;

    private void Awake()
    {
        shieldMatInstance = new Material(shieldMat);
        shader.material = shieldMatInstance;

        dissolveEffect = new DissolveEffect(body.material, Entity.Get<EnemyHealth>().DeathDuration);

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

    public void Appear()
    {
        dissolveEffect.Appear();
    }

    private void OnDestroy()
    {
        Entity.Get<EnemyHealth>().OnLethal -= Vanish;
    }
}
