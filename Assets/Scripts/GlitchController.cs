using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchController : MonoBehaviour
{
    [Header("Glitch Parameters")]
    public float transitionTime;
    public float transitionPauseTime;
    public Color startColor;
    public Color endColor;

    [Header("Material Properties")]
    public Material mat;
    public float noiseAmount;
    public float glitchStrength;
    public float scanLineStrength;
    public float vignettePower;
    public Color vignetteColor;

    private float startNoiseAmount = 0;
    private float endNoiseAmount = 100;
    private float startNoiseStrength = 0;
    private float endNoiseStrength = 100;
    private float startScanLineStrength = 1;
    private float endScanLineStrength = 0;
    private float startVignettePower = 13.34f;
    private float endVignettePower = 3.9f;

    private bool isGlitching = false;
    private Coroutine stopCoroutine;

    private void Start()
    {
        StartCoroutine(StopGlitch());
    }

    private IEnumerator StartGlitch()
    {
        if (stopCoroutine != null)
            StopCoroutine(stopCoroutine);
        isGlitching = true;
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            noiseAmount = Mathf.Lerp(startNoiseAmount, endNoiseAmount, elapsedTime);
            glitchStrength = Mathf.Lerp(startNoiseStrength, endNoiseStrength, elapsedTime);
            scanLineStrength = Mathf.Lerp(startScanLineStrength, endScanLineStrength, elapsedTime);
            vignettePower = Mathf.Lerp(startVignettePower, endVignettePower, elapsedTime);
            vignetteColor = Color.Lerp(startColor, endColor, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(endVignettePower);
        stopCoroutine = StartCoroutine(StopGlitch());
    }

    private IEnumerator StopGlitch()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            noiseAmount = Mathf.Lerp(endNoiseAmount, startNoiseAmount, elapsedTime);
            glitchStrength = Mathf.Lerp(endNoiseStrength, startNoiseStrength, elapsedTime);
            scanLineStrength = Mathf.Lerp(endScanLineStrength, startScanLineStrength, elapsedTime);
            vignettePower = Mathf.Lerp(endVignettePower, startVignettePower, elapsedTime);
            vignetteColor = Color.Lerp(endColor, startColor, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isGlitching = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGlitching)
        {
            mat.SetFloat("_NoiseAmount", noiseAmount);
            mat.SetFloat("_GlitchStrength", glitchStrength);
            mat.SetFloat("_ScanLinesStrength", scanLineStrength);
            mat.SetFloat("_VignettePower", vignettePower);
            mat.SetColor("_VignetteColor", vignetteColor);
        }
    }

    public void TriggerCaughtShader()
    {
        StartCoroutine(StartGlitch());
    }
}
