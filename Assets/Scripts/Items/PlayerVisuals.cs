using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVisuals : Component
{
    [field: SerializeField] public Light2D Light { get; private set; }
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private SpriteRenderer playerRenderer;

    private static readonly int PlayerPositionId = Shader.PropertyToID("_PlayerPosition");

    private float _fadeDuration = 0.5f;
    private Tween flashTween;

    private void LateUpdate()
    {
        Shader.SetGlobalVector(PlayerPositionId, playerRenderer.transform.position);
    }

    public void SetVisible(bool visible)
    {
        playerRenderer.enabled = visible;
    }

    public void Flash(Color color)
    {
        flashTween?.Kill();
        playerRenderer.color = Color.white;
        flashTween = playerRenderer.DOColor(color, 0.1f).SetLoops(2, LoopType.Yoyo);
    }

    public void EnableTrail()
    {
        trail.Clear();
        trail.enabled = true;
    }    

    public void DisableTrail()
    {
        trail.enabled = false;
    }

    public void FadeOut() => StartCoroutine(FadeTo(0.2f, _fadeDuration));
    public void FadeIn() => StartCoroutine(FadeTo(1f, _fadeDuration));

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        Color original = playerRenderer.color;
        float startAlpha = original.a;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);

            playerRenderer.color = new Color(original.r, original.g, original.b, a);
            yield return null;
        }

        playerRenderer.color = new Color(original.r, original.g, original.b, targetAlpha);
    }
}
