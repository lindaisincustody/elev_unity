using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform heart;
    public Transform player;
    public Transform enemy;
    public int GameLevel = 1;
    public int score;
    public TMP_Text scoreText;


    void Start()
    {
        GameLevel = PlayerPrefs.GetInt(Constants.PlayerPrefs.StrengthLevel, 1);
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

        if (score >= 500)
        {

            GameLevel++;
            PlayerPrefs.SetInt(Constants.PlayerPrefs.StrengthLevel, GameLevel);
            Debug.Log("You Won!");
            BattlePlayerController.isPlaying = false;
            SceneManager.LoadScene(Constants.SceneNames.MainScene);
        }

        if (heartController.hp <= 0)
        {
            Debug.Log("You Lost!");
            BattlePlayerController.isPlaying = false;
            SceneManager.LoadScene(Constants.SceneNames.MainScene);
        }

    }

    IEnumerator InstantiateProjectiles()
    {
        EnemyController enemyController = enemy.GetComponent<EnemyController>(); // Reference to the enemy controller

        while (true)
        {
            // Wait until the enemy reaches its target position
            yield return new WaitUntil(() => Vector2.Distance(enemyController.transform.position, enemyController.targetPosition) > 17f);

            // Instantiate a projectile targeting the heart or player
            GameObject projectile = Instantiate(projectilePrefab, enemyController.transform.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.target = (Random.value > 0.5f) ? heart : player; // Randomize target

            // Wait before checking again
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
