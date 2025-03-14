using System.Collections;
using UnityEngine;
using TMPro;

public class NPCTypewriterWithSound : MonoBehaviour
{
    [SerializeField] private float letterDelay = 0.01f; // Delay between each letter.

    [SerializeField]
    private SoundManager.Sound typingSound = SoundManager.Sound.Typing; // Set the typing sound from your SoundManager.

    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Plays the typewriter effect for the given message.
    /// </summary>
    /// <param name="message">The full text to display.</param>
    public void PlayTypewriterEffect(string message)
    {
        StopAllCoroutines();
        StartCoroutine(TypeMessage(message));
    }

    private IEnumerator TypeMessage(string message)
    {
        textComponent.text = "";
        textComponent.gameObject.SetActive(true);
        SoundManager.PlayLoopedSound(typingSound);

        // Split the message into words so that we can handle formatting (e.g. color tags) if needed.
        string[] words = message.Split(' ');
        foreach (string word in words)
        {
            // If the word contains a "color" keyword (for formatting), just append it as-is.
            if (word.Contains("color"))
            {
                textComponent.text += word + " ";
            }
            else
            {
                foreach (char letter in word)
                {
                    textComponent.text += letter;
                    yield return new WaitForSeconds(letterDelay);
                }

                textComponent.text += " ";
            }
        }

        SoundManager.StopLoopedSound(typingSound);
    }
}