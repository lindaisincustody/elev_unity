using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityScreenOverlay : MonoBehaviour
{
    public CanvasGroup overlayCanvasGroup;
    public float maxAlpha = 0.5f;
    public float flickerSpeed = 1.0f;

    private void Update()
    {
        if (SanityBar.instance.currentSanity <= 50)
        {
            overlayCanvasGroup.alpha = Mathf.PingPong(Time.time * flickerSpeed, maxAlpha);
        }
        else
        {
            overlayCanvasGroup.alpha = 0f; // Hide overlay
        }
    }
}