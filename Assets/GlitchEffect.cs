using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    public Material glitchMaterial;
    public float maxGlitchAmount = 1.0f;
    public float glitchSpeed = 1.0f;
    private float glitchAmount;

    private void Update()
    {
        if (SanityBar.instance.currentSanity <= 50)
        {
            glitchAmount = Mathf.PingPong(Time.time * glitchSpeed, maxGlitchAmount);
            glitchMaterial.SetFloat("_GlitchAmount", glitchAmount);
        }
        else
        {
            glitchMaterial.SetFloat("_GlitchAmount", 0f);
        }
    }
}
