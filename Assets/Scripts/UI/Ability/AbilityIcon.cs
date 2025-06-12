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
    [SerializeField] private Image background_passive;
    [SerializeField] private Image glyphHolder;
    [SerializeField] private TextMeshProUGUI glyphText;

    private Ability ability;

    public void Init(Ability ability)
    {
        this.ability = ability;
        
        const float ActiveScale = 0.65f;
        const float PassiveScale = 0.50f;

        float targetScale = (ability.Type == AbilityType.Passive)
        ? PassiveScale
        : ActiveScale;

        GetComponent<RectTransform>().localScale = Vector3.one * targetScale;

        timerText.enabled = false;
        cooldownImage.fillAmount = 0f;
        activeImage.fillAmount = 0f;
       
        abilityIcon.sprite = ability.icon;

        ability.OnActivate += ShowwActive;
        ability.OnCooldown += ShowwCooldown;

        if (ability.Type == AbilityType.Passive)
        {
            glyphHolder.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
            background_passive.gameObject.SetActive(true);
            //background.color = Color.black;
            
        }
        else if(ability.Type == AbilityType.Active)
        {
            glyphHolder.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
            background_passive.gameObject.SetActive(false);
            glyphText.text = ability.Glyph;

        }
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