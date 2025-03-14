using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SanityBar : MonoBehaviour
{
    [field: SerializeField] public SanityEffectHandler sanityEffectHandler { get; private set; }
    public static SanityBar instance;

    public delegate void SanityChangeHandler(int amount);

    public event SanityChangeHandler OnSanityChange;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DecreaseSanityBy50();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddSanity(50);
        }
    }

    public void AddSanity(int amount)
    {
        currentSanity += amount;
        if (currentSanity > maxSanity)
            currentSanity = maxSanity;

        OnSanityChange?.Invoke(amount);
        UpdateSanityUI();
    }

    public void SanityToMin()
    {
        DOVirtual.Int(currentSanity, 0, 150, (val) =>
        {
            currentSanity = val;
            OnSanityChange?.Invoke(currentSanity);
            UpdateSanityUI();
        }).SetSpeedBased(true);
    }

    public void SanityToMax()
    {
        DOVirtual.Int(currentSanity, maxSanity, 150, (val) =>
        {
            currentSanity = val;
            OnSanityChange?.Invoke(currentSanity);
            UpdateSanityUI();
        }).SetSpeedBased(true);
    }

    public void DecreaseSanityBy50()
    {
        currentSanity -= 50;
        if (currentSanity < 0)
            currentSanity = 0;

        OnSanityChange?.Invoke(-50);
        UpdateSanityUI();
    }

    private void UpdateSanityUI()
    {
        float fillAmount = (float)currentSanity / maxSanity;
        mask.fillAmount = fillAmount;
    }
}