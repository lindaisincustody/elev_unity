using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    [SerializeField] private Image mask;

    private void OnEnable()
    {
        SanityManager.Instance.OnSanityChanged += Display;
        Display(SanityManager.Instance.CurrentSanity);
    }

    private void OnDisable()
    {
        SanityManager.Instance.OnSanityChanged -= Display;
    }

    private void Display(int currentSanity)
    {
        mask.fillAmount = (float)currentSanity / SanityManager.Instance.MaxSanity;
    }
}
