using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionEffect : MonoBehaviour
{
    public Material distortionMaterial;
    public float maxDistortion = 0.1f;
    public float distortionSpeed = 1.0f;

    private void Update()
    {
        if (SanityBar.instance.currentSanity <= 50)
        {
            float distortion = Mathf.PingPong(Time.time * distortionSpeed, maxDistortion);
            distortionMaterial.SetFloat("_Distortion", distortion);
        }
        else
        {
            distortionMaterial.SetFloat("_Distortion", 0f);
        }
    }
}

