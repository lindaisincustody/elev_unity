using UnityEngine;

public sealed class WwiseAudioManager : MonoBehaviour
{
    public static WwiseAudioManager Instance { get; private set; }

    [Header("Banks (no extension)")]
    [SerializeField] private string initBank = "Init";
    [SerializeField] private string[] additionalBanks = { "Main", "Music" };

    [Header("Events")]
    [SerializeField] private string playMusicEvent = "Play_Music";

    [Header("RTPC")]
    [SerializeField] private string sanityRtpc = "RTPC_Sanity";
    [SerializeField] private int sanityMin = 0;
    [SerializeField] private int sanityMax = 300;
    [SerializeField] private float rtpcMin = 0f;
    [SerializeField] private float rtpcMax = 100f;

    [Header("Debug")]
    [SerializeField] private bool setRtpcGlobally = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (GetComponent<AkGameObj>() == null)
            gameObject.AddComponent<AkGameObj>();
    }

    private void OnEnable()
    {
        if (SanityBar.instance != null)
            SanityBar.instance.OnSanityValueChanged += OnSanityChanged;
    }

    private void OnDisable()
    {
        if (SanityBar.instance != null)
            SanityBar.instance.OnSanityValueChanged -= OnSanityChanged;
    }

    private void Start()
    {
        Debug.Log($"[Wwise] IsInitialized={AkSoundEngine.IsInitialized()}");

        LoadBankOrLog(initBank);

        foreach (var bank in additionalBanks)
            LoadBankOrLog(bank);

        uint playingId = AkSoundEngine.PostEvent(playMusicEvent, gameObject);
        Debug.Log($"[Wwise] PostEvent '{playMusicEvent}' playingId={playingId}");

        if (SanityBar.instance != null)
            OnSanityChanged(SanityBar.instance.currentSanity);
    }

    private void OnSanityChanged(int sanity)
    {
        float t = Mathf.InverseLerp(sanityMin, sanityMax, sanity);
        float rtpcValue = Mathf.Lerp(rtpcMin, rtpcMax, t);

       
            AkSoundEngine.SetRTPCValue(sanityRtpc, rtpcValue, gameObject);

        Debug.Log($"[Wwise] {sanityRtpc}={rtpcValue:0.00} sanity={sanity} scope={(setRtpcGlobally ? "Global" : "GameObject")}");
    }

    private static void LoadBankOrLog(string bankName)
    {
        if (string.IsNullOrWhiteSpace(bankName))
            return;

        uint bankId;
        var res = AkSoundEngine.LoadBank(bankName, out bankId);
        Debug.Log($"[Wwise] LoadBank '{bankName}' -> {res} (id={bankId})");
    }
}
