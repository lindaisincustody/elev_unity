using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowCircleManager : MonoBehaviour
{
    public GameObject hollowCirclePrefab;

    private int circlesToSpawn = 1;
    private int circlesHit = 0;
    private int levelsToBeat = 3;
    private int levelsBeat = 0;
    private List<GameObject> activeCircles = new List<GameObject>();
    private float minDistanceBetweenCircles = 5.0f; 
    private Animator animator;

    private System.Action Complete;

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.enabled = false;
    }

    public void ActivateGame(int levels, System.Action onCompelete)
    {
        circlesToSpawn = 1;
        circlesHit = 0;
        levelsBeat = 0;
        Complete = onCompelete;
        levelsToBeat = levels;
        SpawnHollowCircles(circlesToSpawn);
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
        circlesHit = 0;
        circlesToSpawn++;
        levelsBeat++;
        if (levelsToBeat == levelsBeat)
            Complete?.Invoke();
        else
            SpawnHollowCircles(circlesToSpawn);
    }

    void SpawnHollowCircles(int numberOfCircles)
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            for (int j = 0; j < 5; j++)
            {
                if (IsTooCloseToExistingCircles(spawnPosition))
                    spawnPosition = GetRandomSpawnPosition();
                else break;
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
        circlesToSpawn = 1;
        circlesHit = 0;
        levelsBeat = 0;
        activeCircles.ForEach(Destroy);
        activeCircles.Clear();
        SpawnHollowCircles(circlesToSpawn);
    }

    public void MissAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger("Ring_Miss");
        ResetGameToLevel1();
    }

    public void TwitchAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger("Ring_tw");
    }

}