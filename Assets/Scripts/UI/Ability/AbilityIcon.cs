using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Image activeImage;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image background;

    private Ability ability;

    public void Init(Ability ability)
    {
        this.ability = ability;

        timerText.enabled = false;
        cooldownImage.fillAmount = 0f;
        activeImage.fillAmount = 0f;

        abilityIcon.sprite = ability.icon;

        if (ability.Type == AbilityType.Passive)
            background.color = Color.black;

        ability.OnActivate += ShowwActive;
        ability.OnCooldown += ShowwCooldown;
    }

    private void ShowwCooldown()
    {
        timerText.enabled = true;
        float cooldownTime = ability.cooldownTime;
        timerText.text = Mathf.CeilToInt(cooldownTime).ToString() + "s";
        cooldownImage.fillAmount = 1f;

        cooldownImage.DOFillAmount(0f, cooldownTime).SetEase(Ease.Linear);

        DOVirtual.Int((int)cooldownTime, 0, cooldownTime, value => { timerText.text = (value + 1) + "s"; })
            .OnComplete(() => timerText.enabled = false);
    }

    private void ShowwActive()
    {
        timerText.enabled = true;
        float cooldownTime = ability.activeTime;
        timerText.text = Mathf.CeilToInt(cooldownTime).ToString() + "s";
        activeImage.fillAmount = 1f;

        activeImage.DOFillAmount(0f, cooldownTime).SetEase(Ease.Linear);

        DOVirtual.Int((int)cooldownTime, 0, cooldownTime, value => { timerText.text = (value + 1) + "s"; })
            .OnComplete(() => timerText.enabled = false);
    }

    private void OnDestroy()
    {
        ability.OnActivate -= ShowwActive;
        ability.OnCooldown -= ShowwCooldown;
    }
}