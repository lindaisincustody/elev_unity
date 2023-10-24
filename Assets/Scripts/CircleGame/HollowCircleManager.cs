using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowCircleManager : MonoBehaviour
{
    public GameObject hollowCirclePrefab;
    private int currentLevel = 0; 
    private int circlesToSpawn = 0; // Number of circles to spawn in the current level.
    private int circlesHit = 0; // Number of circles hit in the current level.
    private List<GameObject> activeCircles = new List<GameObject>();
    private float minDistanceBetweenCircles = 5.0f; 
    private Animator animator;
    private CircleMovement playerCircle;

    void Start()
    {
        ProgressToNextLevel();

        animator = GetComponent<Animator>();

        animator.enabled = false;
    }

    public void TwitchAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger("Ring_tw");

    }

    public void RemoveHollowCircle(GameObject hollowCircle)
    {
        if (activeCircles.Contains(hollowCircle))
        {
            int index = activeCircles.IndexOf(hollowCircle);
            activeCircles.RemoveAt(index);
            Destroy(hollowCircle);

            circlesHit++;

            if (circlesHit == circlesToSpawn)
            {
                ProgressToNextLevel();
            }
        }
    }

    void ProgressToNextLevel()
    {
        currentLevel++;
        circlesHit = 0;

        if (currentLevel == 1)
        {
            circlesToSpawn = 1;
        }
        else if (currentLevel == 2)
        {
            circlesToSpawn = 2;
        }
        else if (currentLevel == 3)
        {
            circlesToSpawn = 3;
        }
        else if (currentLevel == 4)
        {
            circlesToSpawn = 4;
        }
        else if (currentLevel == 5)
        {

            Debug.Log("Game finished!");
        }
        else
        {
            // more levels
        }

        if (currentLevel <= 4)
        {
            SpawnHollowCircles(circlesToSpawn);
        }
    }

    void SpawnHollowCircles(int numberOfCircles)
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            while (IsTooCloseToExistingCircles(spawnPosition))
            {
                spawnPosition = GetRandomSpawnPosition();
            }

            GameObject circle = Instantiate(hollowCirclePrefab, spawnPosition, Quaternion.identity);
            HollowCircle circleScript = circle.GetComponent<HollowCircle>();

            if (circleScript != null)
            {
                circleScript.Initialize(this); 
                activeCircles.Add(circle);
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0f, 360f); // Generate a random angle between 0 and 360 degrees.
        float radius = 5f; 

        Vector3 position = Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
        return position;
    }

    bool IsTooCloseToExistingCircles(Vector3 position)
    {
        foreach (var circle in activeCircles)
        {
            if (Vector3.Distance(position, circle.transform.position) < minDistanceBetweenCircles)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetGameToLevel1()
    {
        currentLevel = 0;
        circlesToSpawn = 1;
        circlesHit = 0;
        activeCircles.ForEach(Destroy);
        activeCircles.Clear();
        ProgressToNextLevel();
    }
    public void MissAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger("Ring_Miss");
    }
}