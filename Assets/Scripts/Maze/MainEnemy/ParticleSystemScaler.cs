using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemScaler : MonoBehaviour
{
    private float[] endSize;
    private float[] startSize;
    private float[] originalSize;
    private ParticleSystem.MainModule[] mainModules; // Array to store the main modules
    public float transitionTime = 5f; // Time in seconds for the transition from 0 to start size and vice versa 

    public void StoreOriginalValues(ParticleSystem[] particles)
    {
        originalSize = new float[particles.Length];
        endSize = new float[particles.Length];
        startSize = new float[particles.Length];
        mainModules = new ParticleSystem.MainModule[particles.Length];

        for (int i = 0; i < particles.Length; i++)
        {
            mainModules[i] = particles[i].main;
            originalSize[i] = mainModules[i].startSize.constant;
        }
    }

    public void SetParticleSizes(bool isExpading, System.Action onTransitionComplete)
    {
        for (int i = 0; i < mainModules.Length; i++)
        {
            if (isExpading)
            {
                startSize[i] = 0;
                endSize[i] = originalSize[i];
                mainModules[i].startSize = 0;
            }
            else
            {
                startSize[i] = originalSize[i];
                endSize[i] = 0;
            }
        }
        if (this != null)
            StartCoroutine(ChangeParticleSizes(isExpading, onTransitionComplete));
    }

    private IEnumerator ChangeParticleSizes(bool isExpading, System.Action onTransitionComplete)
    {
        if (isExpading)
            yield return new WaitForSeconds(transitionTime);

        float elapsedTime = 0f;
        
        while (elapsedTime < transitionTime)
        {
            for (int i = 0; i < mainModules.Length; i++)
            {
                float newSize = Mathf.Lerp(startSize[i], endSize[i], elapsedTime / transitionTime);
                mainModules[i].startSize = newSize;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure that the final size is set to the target size
        for (int i = 0; i < mainModules.Length; i++)
        {
            mainModules[i].startSize = endSize[i];
        }

        onTransitionComplete?.Invoke();
    }
}
