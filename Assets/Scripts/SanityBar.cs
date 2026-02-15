using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SanityBar : MonoBehaviour
{
    [field: SerializeField] public SanityEffectHandler sanityEffectHandler { get; private set; }
    public static SanityBar instance;

    public delegate void SanityValueChangedHandler(int currentSanity);
    public event SanityValueChangedHandler OnSanityValueChanged;

    public Image mask;
    public Image fill;
    public Color color;

    public int currentSanity = 300;
    public int maxSanity = 300;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateSanityUI();
        NotifySanityChanged();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DecreaseSanity(50);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddSanity(50);
        }
    }

    public void AddSanity(int amount)
    {
        currentSanity = Mathf.Clamp(currentSanity + amount, 0, maxSanity);
        UpdateSanityUI();
        NotifySanityChanged();
    }

    public void DecreaseSanity(int amount)
    {
        currentSanity = Mathf.Clamp(currentSanity - amount, 0, maxSanity);
        UpdateSanityUI();
        NotifySanityChanged();
    }

    public void SanityToMin()
    {
        DOVirtual.Int(currentSanity, 0, 150, val =>
        {
            currentSanity = val;
            UpdateSanityUI();
            NotifySanityChanged();
        }).SetSpeedBased(true);
    }

    public void SanityToMax()
    {
        DOVirtual.Int(currentSanity, maxSanity, 150, val =>
        {
            currentSanity = val;
            UpdateSanityUI();
            NotifySanityChanged();
        }).SetSpeedBased(true);
    }

    private void NotifySanityChanged()
    {
        OnSanityValueChanged?.Invoke(currentSanity);
    }

    private void UpdateSanityUI()
    {
        float fillAmount = (float)currentSanity / maxSanity;
        mask.fillAmount = fillAmount;
    }
}
