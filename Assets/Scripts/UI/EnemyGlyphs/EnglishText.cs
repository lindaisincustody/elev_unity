using System.Collections;
using TMPro;
using UnityEngine;

public class EnglishText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float typeSpeed = 0.05f; // Speed of typing effect

    public void WriteText(string poemText, System.Action OnComplete)
    {
        StopAllCoroutines(); // Stop any previous typing effect
        StartCoroutine(TypeText(poemText, OnComplete));
    }

    private IEnumerator TypeText(string poemText, System.Action OnComplete)
    {
        text.text = ""; // Clear previous text
        foreach (char letter in poemText)
        {
            text.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(typeSpeed); // Wait before adding next letter
        }

        OnComplete?.Invoke();
    }
}
