using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseSanityRtpcBinder : MonoBehaviour
{
    [Header("Wwise")]
    [SerializeField] private string rtpcName = "RTPC_Sanity";

    [Header("Mapping")]
    [SerializeField] private int sanityMin = 0;
    [SerializeField] private int sanityMax = 300;
    [SerializeField] private float rtpcMin = 0f;
    [SerializeField] private float rtpcMax = 100f;

    private void OnEnable()
    {
        if (SanityBar.instance != null)
            SanityBar.instance.OnSanityValueChanged += HandleSanityChanged;

        if (SanityBar.instance != null)
            HandleSanityChanged(SanityBar.instance.currentSanity);
    }

    private void OnDisable()
    {
        if (SanityBar.instance != null)
            SanityBar.instance.OnSanityValueChanged -= HandleSanityChanged;
    }

    private void HandleSanityChanged(int currentSanity)
    {
        float t = Mathf.InverseLerp(sanityMin, sanityMax, currentSanity);
        float rtpcValue = Mathf.Lerp(rtpcMin, rtpcMax, t);

       // AkSoundEngine.SetRTPCValue("RTPC_Sanity", rtpcValue, gameObject);
    }
}
