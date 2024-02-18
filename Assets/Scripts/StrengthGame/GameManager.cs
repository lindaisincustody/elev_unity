using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform heart;
    public int GameLevel = 1;
    public int score;
    public TMP_Text scoreText;

    void Start()
    {
        // Start the coroutine to instantiate projectiles
        StartCoroutine(InstantiateProjectiles());

    }

    private void Update()
    {
        CheckGameCondition();
        UpdateCurrentLevelsText();
    }
    
    void UpdateCurrentLevelsText()
    {
        // Update the TMP text to display the current value of wonLevels
        scoreText.text = "Score: " + score.ToString();
    }
    void CheckGameCondition()
    {
        Heart heartController = FindObjectOfType<Heart>();

        if (score >= 3)
        {

            GameLevel++;
            Debug.Log("You Won!");
            SceneManager.LoadScene("SampleScene");
        }

        if (heartController.hp <= 0)
        {
            Debug.Log("You Lost!");
            SceneManager.LoadScene("SampleScene");
        }

    }

    IEnumerator InstantiateProjectiles()
    {
        while (true)
        {
            // Determine the number of projectiles based on the GameLevel
            int projectilesPerLevel = GameLevel; // Example formula: 2 projectiles per level

            for (int i = 0; i < projectilesPerLevel; i++)
            {
                // Alternate directions for each projectile
                Vector2 direction = i % 2 == 0 ? Vector2.left : Vector2.right;
                GameObject projectile = Instantiate(projectilePrefab, GetSpawnPosition(direction), Quaternion.identity);
                projectile.GetComponent<Projectile>().target = heart;

                // Short delay between each projectile instantiation
                yield return new WaitForSeconds(0.5f);
            }

            // Longer delay after all projectiles for a level have been instantiated
            yield return new WaitForSeconds(2f);
        }
    }


    Vector2 GetSpawnPosition(Vector2 direction)
    {
        float spawnX = (direction == Vector2.left) ? -10f : 10f;
        float spawnY = Random.Range(-5f, 5f);
        return new Vector2(spawnX, spawnY);
    }
}
