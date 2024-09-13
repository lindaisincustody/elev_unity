using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal; // Needed for Light2D

public class SanityEffectHandler : MonoBehaviour
{
    public Volume globalVolume;  // The post-processing volume
    public Light2D sanityLight;  // Reference to the 2D light component
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;
    private FilmGrain filmGrain;
    private Bloom bloom;
    private MotionBlur motionBlur;
    private DepthOfField depthOfField;
    private float timeElapsed;
    public Material rippleMaterial;  // Reference to the ripple material

    public static bool IsPlayerInUnderworld { get; private set; } // Flag to track if player is in the underworld

    private bool isRippleActive = false;
    private bool isAnimating = false; // Keep track of animation state

    private void Start()
    {
        if (SanityBar.instance != null)
        {
            SanityBar.instance.OnSanityChange += OnSanityChange;
        }

        LoadVolumeComponents();
        ResetEffects();
        IsPlayerInUnderworld = false; // Ensure it starts as false
    }

    private void Update()
    {
        // Check if the "1" key is pressed
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
        int currentSanity = Mathf.Max(SanityBar.instance.currentSanity, 0);  // Clamp sanity at 0

        if (sanityLight == null) return;

        if (currentSanity <= 50 && !isRippleActive && !isAnimating)
        {
            sanityLight.color = new Color32(0x00, 0xE7, 0xFF, 0xFF);  // Blue light
            sanityLight.intensity = 2f;
            IsPlayerInUnderworld = true; // Set the player to be in the underworld
            isAnimating = true;
            StartCoroutine(AnimateSchizophrenicEffects());
        }
        else if (currentSanity > 50)
        {
            sanityLight.color = new Color32(0xF8, 0xC4, 0x64, 0xFF);  // Yellow light
            sanityLight.intensity = 1f;
            IsPlayerInUnderworld = false; // Player exits the underworld
            StopAllCoroutines();  // Stop the animation if sanity goes back up
            ResetEffects();
            isAnimating = false;
        }
    }

    private void IncreaseSanityBy50()
    {
        if (SanityBar.instance != null)
        {
            SanityBar.instance.AddSanity(50); // Increase sanity by 50
        }
    }

    private IEnumerator AnimateSchizophrenicEffects()
    {
        timeElapsed = 0;  // Reset timeElapsed when animation starts

        while (SanityBar.instance.currentSanity <= 50)  // Keep animating as long as sanity is 50 or less
        {
            timeElapsed += Time.deltaTime;

            // Animate the effects
            AnimateEffect(vignette, v => v.intensity.value = Mathf.Lerp(0.6f, 0.9f, (Mathf.Sin(timeElapsed * 2f) + 1f) / 2f));
            AnimateEffect(chromaticAberration, c => c.intensity.value = Mathf.Lerp(0.7f, 1.0f, Mathf.PerlinNoise(timeElapsed, 0.0f)));
            AnimateEffect(colorAdjustments, c =>
            {
                c.saturation.value = Mathf.Lerp(-100f, -50f, (Mathf.Sin(timeElapsed * 1.5f) + 1f) / 2f);
                c.hueShift.value = Mathf.Lerp(-20f, -10f, Mathf.PerlinNoise(0.0f, timeElapsed));
                c.colorFilter.value = Color.Lerp(new Color(0.5f, 0f, 0f), Color.red, Mathf.PerlinNoise(timeElapsed * 0.5f, timeElapsed * 0.5f));
            });
            AnimateEffect(filmGrain, f => f.intensity.value = Mathf.Lerp(0.4f, 1.0f, Mathf.PerlinNoise(timeElapsed * 0.5f, 0.0f)));
            AnimateEffect(bloom, b => b.intensity.value = Mathf.Lerp(0.1f, 0.3f, Mathf.PerlinNoise(timeElapsed * 0.3f, timeElapsed * 0.3f)));
            AnimateEffect(motionBlur, m => m.intensity.value = Mathf.Lerp(0.5f, 1.0f, Mathf.PerlinNoise(timeElapsed * 1f, 0.0f)));
            AnimateEffect(depthOfField, d => d.focusDistance.value = Mathf.Lerp(10f, 0.1f, Mathf.Sin(timeElapsed * 0.5f)));

            yield return null;  // Wait for the next frame
        }

        isAnimating = false; // Stop the animation when sanity is increased again
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
        ResetEffect(bloom, b => b.intensity.value = 0.5f);
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
        globalVolume.profile.TryGet(out bloom);
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
