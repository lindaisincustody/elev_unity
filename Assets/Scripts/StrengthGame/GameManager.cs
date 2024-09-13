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

    private DataManager dataManager;

    void Start()
    {
        dataManager = DataManager.Instance;

        GameLevel = dataManager.GetLevel(Attribute.Strength);
        // Start the coroutine to instantiate projectiles
  

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
            dataManager.AddLevel(Attribute.Strength);
            Debug.Log("You Won!");
            BattlePlayerController.isPlaying = false;
            SceneManager.LoadScene(DataManager.Instance.GetLastScene());
        }

        if (heartController.hp <= 0)
        {
            Debug.Log("You Lost!");
            BattlePlayerController.isPlaying = false;
            SceneManager.LoadScene(DataManager.Instance.GetLastScene());
        }

    }




    Vector2 GetSpawnPosition(Vector2 direction)
    {
        float spawnX = (direction == Vector2.left) ? -10f : 10f;
        float spawnY = Random.Range(-5f, 5f);
        return new Vector2(spawnX, spawnY);
    }
}
