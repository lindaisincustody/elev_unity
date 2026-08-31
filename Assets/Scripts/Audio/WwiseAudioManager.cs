using Cysharp.Threading.Tasks;
using UnityEngine;

public class WwiseAudioManager : CoreService
{
    public static WwiseAudioManager Instance { get; private set; }

    [SerializeField] private string initBank = "Init";
    [SerializeField] private string[] additionalBanks = { "Main", "Music" };
    [SerializeField] private string playMusicEvent = "Play_Music";
    [SerializeField] private string sanityRtpc = "RTPC_Sanity";
    [SerializeField] private int sanityMin = 0;
    [SerializeField] private int sanityMax = 300;
    [SerializeField] private float rtpcMin = 0f;
    [SerializeField] private float rtpcMax = 100f;
    [SerializeField] private bool setRtpcGlobally = true;

    public override UniTask Initialize()
    {
        Instance = this;

        LoadBank(initBank);

        foreach (string bank in additionalBanks)
            LoadBank(bank);

        SanityManager.Instance.OnSanityChanged += OnSanityChanged;
        OnSanityChanged(SanityManager.Instance.CurrentSanity);

        return UniTask.CompletedTask;
    }

    private void OnDestroy()
    {
        SanityManager.Instance.OnSanityChanged -= OnSanityChanged;
    }

    public void PlayMusic()
    {
        // AkSoundEngine.PostEvent(playMusicEvent, gameObject);
    }

    private void OnSanityChanged(int sanity)
    {
        float t = Mathf.InverseLerp(sanityMin, sanityMax, sanity);
        float rtpcValue = Mathf.Lerp(rtpcMin, rtpcMax, t);

        // AkSoundEngine.SetRTPCValue(sanityRtpc, rtpcValue, gameObject);
    }

    private void LoadBank(string bankName)
    {
        // AkSoundEngine.LoadBank(bankName, out uint bankId);
    }
}
