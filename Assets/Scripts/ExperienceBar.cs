using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public static ExperienceBar instance;

    public delegate void ExperienceChangeHandler(int amount);
    public event ExperienceChangeHandler OnExperienceChange;

    public Image mask;
    public Image fill;
    public Color color;

    public int currentExperience = 0;
    public int maxExperience = 300;


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

    public void AddExperience(int amount)
    {
        OnExperienceChange?.Invoke(amount);
        UpdateExperienceUI();
    }

    private void Update()
    {
        UpdateExperienceUI();
    }

    private void UpdateExperienceUI()
    {
        //PlayerData playerData = Player.instance?.playerData; // Access player data from the Player singleton

            float currentOffset = currentExperience;
            float maximumOffset = maxExperience;
            float fillAmount = currentOffset / maximumOffset;
            mask.fillAmount = fillAmount;

    }
}
