using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private EnemyLeash leashController;
    [SerializeField] private Material wallsMat;
    [SerializeField] private Material BeamEffectMat;
    [SerializeField] private Material GlitchEffectMat;

    public float leaseEffectTime = 7f;

    private bool leashAffectTime = false;

    private void Start()
    {
        ResetEffects();
    }

    private void ResetEffects()
    {
        float factor = Mathf.Pow(2, 8);
        Color color = new Color(0.8f * factor, 0.01f * factor, 0.01f * factor);
        BeamEffectMat.SetColor("_Color", color);

        wallsMat.SetFloat("_Dissolve", 0);

        Color glitchDefaultColor = new Color(0f, 0.7490194f, 0.7490194f);
        GlitchEffectMat.SetFloat("_NoiseAmount", 0);
        GlitchEffectMat.SetFloat("_GlitchStrength", 0);
        GlitchEffectMat.SetFloat("_ScanLinesStrength", 1);
        GlitchEffectMat.SetFloat("_VignettePower", 13.34f);
        GlitchEffectMat.SetColor("_VignetteColor", glitchDefaultColor);
    }

    public bool ActivateCircleMaskEffect()
    {
        if (!leashAffectTime)
            StartCoroutine(CircleMaskEffect());
        return leashAffectTime;
    }

    private IEnumerator CircleMaskEffect()
    {
        leashAffectTime = true;
        leashController.LeashPlayer();
        yield return new WaitForSeconds(leaseEffectTime);
        leashController.UnleashPlayer();
        yield return new WaitForSeconds(1f);
        leashAffectTime = false;
    }

    public void ActivateDissolveWallsEffect()
    {
        StartCoroutine(DissolveWalls());
    }

    private IEnumerator DissolveWalls()
    {
        float elapsedTime = 0f;

        // Loop over the specified duration
        while (elapsedTime < 7)
        {
            elapsedTime += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0f, 1f, elapsedTime / 3);
            wallsMat.SetFloat("_Dissolve", dissolveValue);
            yield return null;
        }

        wallsMat.SetFloat("_Dissolve", 1f); // Ensure the value is set to 1 at the end
        StartCoroutine(RestoreWalls());
    }

    private IEnumerator RestoreWalls()
    {
        float elapsedTime = 0f;

        // Loop over the specified duration
        while (elapsedTime < 3)
        {
            elapsedTime += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(1f, 0f, elapsedTime / 3);
            wallsMat.SetFloat("_Dissolve", dissolveValue);
            yield return null;
        }

        wallsMat.SetFloat("_Dissolve", 0f); // Ensure the value is set to 1 at the end
    }

}
