using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PatternMatchingGame : MonoBehaviour
{
    public TextMeshProUGUI patternDisplay;
    public TextMeshProUGUI feedbackText;

    private string currentPattern;
    private string playerInput;

    private int gridSize = 3; // 3x3 grid for simplicity
    private float timeBetweenPatterns = 2f;

    void Start()
    {
        GeneratePattern();
        StartCoroutine(ShowPattern());
    }

    void Update()
    {
        // For simplicity, using the space bar as the input trigger
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckPattern();
        }
    }

    IEnumerator ShowPattern()
    {
        patternDisplay.text = currentPattern;
        yield return new WaitForSeconds(timeBetweenPatterns);
        patternDisplay.text = "";
        playerInput = "";
    }

    void GeneratePattern()
    {
        currentPattern = "";
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            // Randomly choose between X and O
            currentPattern += (Random.Range(0, 2) == 0) ? "X" : "O";
        }
    }

    void CheckPattern()
    {
        playerInput += Random.Range(0, 2) == 0 ? "X" : "O";

        if (playerInput.Length == currentPattern.Length)
        {
            if (playerInput == currentPattern)
            {
                feedbackText.text = "Correct!";
                IncreaseDifficulty();
                GeneratePattern();
                StartCoroutine(ShowPattern());
            }
            else
            {
                feedbackText.text = "Incorrect! Game Over.";
                // Handle game over logic here
            }
        }
    }

    void IncreaseDifficulty()
    {
        // Adjust parameters for increased difficulty
        gridSize++;
        timeBetweenPatterns -= 0.1f;
    }
}
