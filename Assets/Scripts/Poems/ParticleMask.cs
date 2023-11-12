using System.Collections;
using UnityEngine;

public class ParticleMask : MonoBehaviour
{
    private float duration;
    public float timePerletter;
    public float decreaseDuration;

    public float startEmissionRate = 100f;
    private ParticleSystem particleSystem;
    private ParticleSystem.EmissionModule emissionModule;
    private float currentTime = 0f;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("No ParticleSystem found on this GameObject.");
            return;
        }

        emissionModule = particleSystem.emission;
        emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(startEmissionRate);

    }

    public void calculateDuration(string word)
    {
        duration = word.Length * timePerletter;
    }

    private void OnEnable()
    {
        StartCoroutine(showParticles());
    }

    private IEnumerator showParticles()
    {
        yield return new WaitForSeconds(duration);
        float elapsedTime = 0f;
        while (elapsedTime < decreaseDuration)
        {
            currentTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;

            // Evaluate the emission rate based on the current time and the emissionRateCurve.
            float evaluatedEmissionRate = Mathf.Lerp(startEmissionRate, 0f, elapsedTime / decreaseDuration);

            // Update the emission rate over time.
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(evaluatedEmissionRate);

            yield return null;
        }

        yield return new WaitForSeconds(4f);
        emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(startEmissionRate);
    }
}
