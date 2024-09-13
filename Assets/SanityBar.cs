using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.Alpha0))  // Listen for number 0 key press
        {
            DecreaseSanityBy50();
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

    private void DecreaseSanityBy50()
    {
        currentSanity -= 50;
        if (currentSanity < 0)
            currentSanity = 0;

        OnSanityChange?.Invoke(-50);  // Invoke with negative amount to indicate decrease
        UpdateSanityUI();
    }

    private void UpdateSanityUI()
    {
        float fillAmount = (float)currentSanity / maxSanity;
        mask.fillAmount = fillAmount;
    }
}
