using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;
using System;

public class SanityEffectHandler : MonoBehaviour
{
    public Volume globalVolume;
    public Light2D sanityLight;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;
    private FilmGrain filmGrain;
    private Bloom bloom;
    private MotionBlur motionBlur;
    public Animator PlayerAnimator;
    private DepthOfField depthOfField;
    private float timeElapsed;
    public Material rippleMaterial;

    private bool _isPlayerInUnderworld;

    public bool IsPlayerInUnderworld
    {
        get => _isPlayerInUnderworld;
        private set
        {
            if (_isPlayerInUnderworld != value)
            {
                _isPlayerInUnderworld = value;
                OnWorldChange?.Invoke();
            }
        }
    }

    public Action OnWorldChange { get; set; }

    private bool isRippleActive = false;
    private bool isAnimating = false;

    private void Awake()
    {
        if (SanityBar.instance != null)
        {
            SanityBar.instance.OnSanityChange += OnSanityChange;
        }
    }

    private void Start()
    {
        LoadVolumeComponents();
        ResetEffects();

        IsPlayerInUnderworld = false;
        PlayerAnimator.SetBool("IsPlayerInUnderworldAnimation", false);

        StartCoroutine(AnimateRealWorldEffects());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            IncreaseSanityBy50();
        }
    }

    private void OnDestroy()
    {
        if (SanityBar.instance != null)
        {
            SanityBar.instance.OnSanityChange -= OnSanityChange;
        }
    }

    private void OnSanityChange(int amount)
    {
        int currentSanity = Mathf.Max(SanityBar.instance.currentSanity, 0);

        if (sanityLight == null) return;

        if (currentSanity <= 50 && !isRippleActive && !isAnimating)
        {
            sanityLight.color = new Color32(0x00, 0xE7, 0xFF, 0xFF);
            sanityLight.intensity = 2f;
            IsPlayerInUnderworld = true;
            PlayerAnimator.SetBool("IsPlayerInUnderworldAnimation", true);
            isAnimating = true;
            StartCoroutine(AnimateSchizophrenicEffects());
        }
        else if (currentSanity > 50)
        {
            sanityLight.color = new Color32(0xF8, 0xC4, 0x64, 0xFF);
            sanityLight.intensity = 1f;
            IsPlayerInUnderworld = false;
            PlayerAnimator.SetBool("IsPlayerInUnderworldAnimation", false);
            isAnimating = false;
            StartCoroutine(AnimateRealWorldEffects());
        }
    }

    private void SetRealWorldEffects()
    {
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = 0.3f;

        if (filmGrain != null)
            filmGrain.intensity.value = 0.5f;

        if (motionBlur != null)
            motionBlur.intensity.value = 0.3f;

        if (depthOfField != null)
            depthOfField.focusDistance.value = 5f;
    }

    private void IncreaseSanityBy50()
    {
        if (SanityBar.instance != null)
        {
            SanityBar.instance.AddSanity(50);
        }
    }

    private IEnumerator AnimateSchizophrenicEffects()
    {
        timeElapsed = 0;

        while (SanityBar.instance.currentSanity <= 50)
        {
            timeElapsed += Time.deltaTime;

            // Animate the effects
            AnimateEffect(vignette,
                v => v.intensity.value = Mathf.Lerp(0.6f, 0.9f, (Mathf.Sin(timeElapsed * 2f) + 1f) / 2f));
            AnimateEffect(chromaticAberration,
                c => c.intensity.value = Mathf.Lerp(0.7f, 1.0f, Mathf.PerlinNoise(timeElapsed, 0.0f)));
            AnimateEffect(colorAdjustments, c =>
            {
                c.saturation.value = Mathf.Lerp(-100f, -50f, (Mathf.Sin(timeElapsed * 1.5f) + 1f) / 2f);
                c.hueShift.value = Mathf.Lerp(-20f, -10f, Mathf.PerlinNoise(0.0f, timeElapsed));
                c.colorFilter.value = Color.Lerp(new Color(0.5f, 0f, 0f), Color.red,
                    Mathf.PerlinNoise(timeElapsed * 0.5f, timeElapsed * 0.5f));
            });
            AnimateEffect(filmGrain,
                f => f.intensity.value = Mathf.Lerp(0.4f, 1.0f, Mathf.PerlinNoise(timeElapsed * 0.5f, 0.0f)));

            AnimateEffect(motionBlur,
                m => m.intensity.value = Mathf.Lerp(0.5f, 1.0f, Mathf.PerlinNoise(timeElapsed * 1f, 0.0f)));
            AnimateEffect(depthOfField,
                d => d.focusDistance.value = Mathf.Lerp(10f, 0.1f, Mathf.Sin(timeElapsed * 0.5f)));

            yield return null;
        }

        isAnimating = false;
    }

    private IEnumerator AnimateRealWorldEffects()
    {
        timeElapsed = 0;

        while (SanityBar.instance.currentSanity > 50)
        {
            timeElapsed += Time.deltaTime;

            AnimateEffect(vignette,
                v => v.intensity.value = Mathf.Lerp(0f, 0f, Mathf.PerlinNoise(timeElapsed, 0.0f)));
            AnimateEffect(chromaticAberration,
                c => c.intensity.value = Mathf.Lerp(0.9f, 1.0f, Mathf.PerlinNoise(timeElapsed, 0.0f)));
            AnimateEffect(colorAdjustments, c =>
            {
                c.saturation.value = Mathf.Lerp(0, 0, (Mathf.Sin(timeElapsed * 0)));
                c.hueShift.value = Mathf.Lerp(0f, 0f, Mathf.PerlinNoise(0.0f, timeElapsed));
                c.colorFilter.value = Color.Lerp(new Color(0f, 0f, 0f), Color.white,
                    Mathf.PerlinNoise(timeElapsed * 0f, timeElapsed * 0f));
            });
            AnimateEffect(filmGrain,
                f => f.intensity.value = Mathf.Lerp(0.1f, 1.0f, Mathf.PerlinNoise(timeElapsed * 0.5f, 0.0f)));

            AnimateEffect(depthOfField,
                d => d.focusDistance.value = Mathf.Lerp(10f, 5f, Mathf.Sin(timeElapsed * 0.5f)));

            yield return null;
        }

        isAnimating = false;
    }

    private void ResetEffects()
    {
        // Use the helper function to reset effects
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
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out colorAdjustments);
        globalVolume.profile.TryGet(out lensDistortion);
        globalVolume.profile.TryGet(out filmGrain);
        globalVolume.profile.TryGet(out motionBlur);
        globalVolume.profile.TryGet(out depthOfField);
    }

    private void AnimateEffect<T>(T effect, System.Action<T> action) where T : VolumeComponent
    {
        if (effect != null)
        {
            action(effect);
        }
    }

    private void ResetEffect<T>(T effect, System.Action<T> action) where T : VolumeComponent
    {
        if (effect != null)
        {
            action(effect);
        }
    }
}