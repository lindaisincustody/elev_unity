using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PillEfect : MonoBehaviour
{
    [SerializeField] private Volume postProcessingVolume;

    public void StartEffect()
    {
        StartCoroutine(ApplyTrippyEffects(60));
    }

    private IEnumerator ApplyTrippyEffects(float duration)
    {
        // Total duration of the effect
        float elapsed = 0f;

        ChromaticAberration chromaticAberration = null;
        LensDistortion lensDistortion = null;

        if (postProcessingVolume.profile.TryGet(out chromaticAberration) &&
            postProcessingVolume.profile.TryGet(out lensDistortion))
        {
            float maxChromaticIntensity = 1f; // Maximum intensity for Chromatic Aberration
            float minLensDistortion = -1f; // Minimum intensity for Lens Distortion
            float maxLensDistortion = 1f; // Maximum intensity for Lens Distortion
            float frequencyMultiplier = 5f; // Oscillation frequency

            while (elapsed < duration)
            {
                float remainingTime = duration - elapsed;
                //effectDurationText.text = $"Effect Duration: {remainingTime.ToString("0.0")}s";  // Update the duration text
                //savedDuration = remainingTime;
                float sinusoidalFactor = Mathf.Sin(2 * Mathf.PI * frequencyMultiplier * elapsed / duration);
                chromaticAberration.intensity.value = (sinusoidalFactor + 1f) / 2 * maxChromaticIntensity;
                lensDistortion.intensity.value = sinusoidalFactor * (maxLensDistortion - minLensDistortion) / 2 + (maxLensDistortion + minLensDistortion) / 2;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Reset effects and UI
            chromaticAberration.intensity.value = 0f;
            lensDistortion.intensity.value = 0f;
           // effectDurationText.text = "Effect Duration: 0.0s";
           // itemIcon.gameObject.SetActive(false);
           // effectDurationText.gameObject.SetActive(false);// Hide the item icon
        }
    }
}
