using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class SanityEffectHandler : MonoBehaviour
{
    public Material rippleMaterial;
    public Material crteffectMaterial;

    private Light2D sanityLight;
    private Animator playerAnimator;

    private Volume globalVolume;

    private DepthOfField depthOfField;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;
    private FilmGrain filmGrain;
    private Bloom bloom;
    private MotionBlur motionBlur;

    private float timeElapsed;

    private float displayedSanity;

    public float transitionSpeed = 3f;

    private Coroutine crteffectCoroutine;

    private bool bound;

    private void Start()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        GameSession.Instance.OnGameStarted += Bind;

        if (GameSession.Instance.IsRunning)
            Bind();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        GameSession.Instance.OnGameStarted -= Bind;
        SanityManager.Instance.OnSanityChanged -= OnSanityChange;
    }

    private void HandleSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (GameSession.Instance.IsRunning)
            Bind();
    }

    private void Bind()
    {
        LoadVolumeComponents();
        ResetEffects();

        playerAnimator = Player.instance.Animator;
        sanityLight = Player.instance.Get<PlayerVisuals>().Light;

        displayedSanity = SanityManager.Instance.CurrentSanity;

        if (!bound)
        {
            bound = true;
            SanityManager.Instance.OnSanityChanged += OnSanityChange;
            StartCoroutine(UpdateEffectsBasedOnSanity());
        }

        OnSanityChange(SanityManager.Instance.CurrentSanity);
    }

    private void OnSanityChange(int amount)
    {
        if (!SanityManager.Instance.IsPlayerInUnderworld)
        {
            sanityLight.color = new Color32(0xF8, 0xC4, 0x64, 0xFF);
            sanityLight.intensity = 1f;
        }
        else
        {
            sanityLight.color = new Color32(0x00, 0xE7, 0xFF, 0xFF);
            sanityLight.intensity = 2f;
        }

        playerAnimator.SetBool("IsPlayerInUnderworldAnimation", SanityManager.Instance.IsPlayerInUnderworld);

        TriggerCRTEffectTransition();
    }

    private IEnumerator UpdateEffectsBasedOnSanity()
    {
        while (true)
        {
            timeElapsed += Time.deltaTime;
            float targetSanity = SanityManager.Instance.CurrentSanity;
            displayedSanity = Mathf.Lerp(displayedSanity, targetSanity, transitionSpeed * Time.deltaTime);

            float sanity = displayedSanity;


            float t = 0f;
            if (sanity > 200)
            {
                t = 0f;
            }
            else if (sanity > 100)
            {
                t = 0.5f * (200 - sanity) / 100f;
            }
            else
            {
                t = 0.5f + 0.5f * ((100 - sanity) / 100f);
            }

            float vignetteReal = 0f;
            float chromaticReal = Mathf.Lerp(0.9f, 1.0f, Mathf.PerlinNoise(timeElapsed, 0f));
            float saturationReal = 0f;
            float hueShiftReal = 0f;
            Color colorFilterReal = Color.white;
            float filmGrainReal = Mathf.Lerp(0.1f, 1.0f, Mathf.PerlinNoise(timeElapsed * 0.5f, 0f));
            float motionBlurReal = 0f;
            float depthOfFieldReal = Mathf.Lerp(10f, 5f, Mathf.Sin(timeElapsed * 0.5f));

            float vignetteSchizo = Mathf.Lerp(0.6f, 0.9f, (Mathf.Sin(timeElapsed * 2f) + 1f) / 2f);
            float chromaticSchizo = Mathf.Lerp(0.7f, 1.0f, Mathf.PerlinNoise(timeElapsed, 0f));
            float saturationSchizo = Mathf.Lerp(-100f, -50f, (Mathf.Sin(timeElapsed * 1.5f) + 1f) / 2f);
            float hueShiftSchizo = Mathf.Lerp(-20f, -10f, Mathf.PerlinNoise(0f, timeElapsed));
            Color colorFilterSchizo = Color.Lerp(new Color(0.5f, 0f, 0f), Color.red,
                Mathf.PerlinNoise(timeElapsed * 0.5f, timeElapsed * 0.5f));
            float filmGrainSchizo = Mathf.Lerp(0.4f, 1.0f, Mathf.PerlinNoise(timeElapsed * 0.5f, 0f));
            float motionBlurSchizo = Mathf.Lerp(0.5f, 1.0f, Mathf.PerlinNoise(timeElapsed, 0f));
            float depthOfFieldSchizo = Mathf.Lerp(10f, 0.1f, Mathf.Sin(timeElapsed * 0.5f));

            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(vignetteReal, vignetteSchizo, t);

            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticReal, chromaticSchizo, t);

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(saturationReal, saturationSchizo, t);
                colorAdjustments.hueShift.value = Mathf.Lerp(hueShiftReal, hueShiftSchizo, t);
                colorAdjustments.colorFilter.value = Color.Lerp(colorFilterReal, colorFilterSchizo, t);
            }

            if (filmGrain != null)
                filmGrain.intensity.value = Mathf.Lerp(filmGrainReal, filmGrainSchizo, t);

            if (motionBlur != null)
                motionBlur.intensity.value = Mathf.Lerp(motionBlurReal, motionBlurSchizo, t);

            if (depthOfField != null)
                depthOfField.focusDistance.value = Mathf.Lerp(depthOfFieldReal, depthOfFieldSchizo, t);

            yield return null;
        }
    }


    private void TriggerCRTEffectTransition()
    {
        if (crteffectMaterial == null)
            return;

        if (crteffectCoroutine != null)
        {
            StopCoroutine(crteffectCoroutine);
        }

        crteffectCoroutine = StartCoroutine(AnimateCRTEffect());
    }


    private IEnumerator AnimateCRTEffect()
    {
        float duration = 1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalized = Mathf.Clamp01(elapsed / duration);

            float f = Mathf.Sin(normalized * Mathf.PI);

            float colorShiftValue = 0.184f + 0.816f * f;

            float speedValue = 0.38f + 0.22f * f;
            crteffectMaterial.SetFloat("_ColorShift", colorShiftValue);
            crteffectMaterial.SetFloat("_Speed", speedValue);
            yield return null;
        }

        crteffectMaterial.SetFloat("_ColorShift", 0.184f);
        crteffectMaterial.SetFloat("_Speed", 0.78f);
        crteffectCoroutine = null;
    }

    private void ResetEffects()
    {
        ResetEffect(vignette, v => v.intensity.value = 0.1f);
        ResetEffect(chromaticAberration, c => c.intensity.value = 0f);
        ResetEffect(colorAdjustments, c =>
        {
            c.saturation.value = 0f;
            c.hueShift.value = 0f;
            c.colorFilter.value = Color.white;
        });
        ResetEffect(filmGrain, f => f.intensity.value = 0f);
        ResetEffect(motionBlur, m => m.intensity.value = 0f);
        ResetEffect(depthOfField, d => d.focusDistance.value = 10f);
    }

    private void LoadVolumeComponents()
    {
        globalVolume = SceneEffectManager.Instance.GlobalVolume;

        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out colorAdjustments);
        globalVolume.profile.TryGet(out lensDistortion);
        globalVolume.profile.TryGet(out filmGrain);
        globalVolume.profile.TryGet(out motionBlur);
        globalVolume.profile.TryGet(out depthOfField);
    }

    private void ResetEffect<T>(T effect, Action<T> action) where T : VolumeComponent
    {
        if (effect != null)
        {
            action(effect);
        }
    }
}