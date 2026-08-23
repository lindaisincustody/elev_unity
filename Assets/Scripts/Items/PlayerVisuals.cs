using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVisuals : Component
{
    [field: SerializeField] public Light2D Light { get; private set; }
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private SpriteRenderer playerRenderer;

    private float _fadeDuration = 0.5f;

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
