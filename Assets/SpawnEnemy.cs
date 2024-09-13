using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;  // Reference to the enemy prefab
    public Vector2 roamingAreaCenter = new Vector2(25f, 1f); // Center of the roaming area
    public Vector2 roamingAreaSize = new Vector2(50f, 5f); // Size of the roaming area (width, height)
    public int numberOfEnemies = 5; // Number of enemies to spawn

    void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Generate a random position within the roaming area
            Vector2 randomPosition = GetRandomPositionWithinArea();

            // Instantiate enemy at the random position
            GameObject newEnemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);

            // Set the roaming area for the enemy
            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.roamingAreaCenter = roamingAreaCenter;
                enemyController.roamingAreaSize = roamingAreaSize;
            }
        }
    }

    private Vector2 GetRandomPositionWithinArea()
    {
        float randomX = Random.Range(roamingAreaCenter.x - roamingAreaSize.x / 2, roamingAreaCenter.x + roamingAreaSize.x / 2);
        float randomY = Random.Range(roamingAreaCenter.y - roamingAreaSize.y / 2, roamingAreaCenter.y + roamingAreaSize.y / 2);
        return new Vector2(randomX, randomY);
    }
}
