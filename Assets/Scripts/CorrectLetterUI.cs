using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CorrectLetterUI : MonoBehaviour
{
    public static CorrectLetterUI Instance { get; private set; }
    public TextMeshProUGUI displayText;
    private Animator animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
    }

    public void Show(string letter)
    {
        displayText.text = $"<color=#37B63B>Correct!</color>";
        displayText.gameObject.SetActive(true);

        // Reset and start the animation
        animator.ResetTrigger("ZoomOut");
        animator.SetTrigger("ZoomIn");

        // Start the coroutine to hide after 1.0 seconds
        StartCoroutine(HideAfterDelay(1.0f));
    }

    public void ShowWrong(string letter)
    {
        displayText.text = $"<color=#CB533E>Wrong!</color>";
        displayText.gameObject.SetActive(true);

        // Reset and start the animation
        animator.ResetTrigger("ZoomOut");
        animator.SetTrigger("ZoomIn");

        // Start the coroutine to hide after 1.0 seconds
        StartCoroutine(HideAfterDelay(1.0f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("ZoomOut");
        displayText.gameObject.SetActive(false);
    }
}