using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowCircleManager : MonoBehaviour
{
    public GameObject hollowCirclePrefab;
    public PolygonCollider2D baseRingCollider; // Reference to the ring collider.

    private int circlesToSpawn = 1;
    private int circlesHit = 0;
    private int levelsToBeat = 3;
    private int levelsBeat = 0;
    private List<GameObject> activeCircles = new List<GameObject>();
    private float minDistanceBetweenCircles = 1.5f; // Adjusted for better spacing
    private Animator animator;
    private System.Action Complete;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void ActivateGame(int levels, System.Action onComplete)
    {
        circlesToSpawn = 1;
        circlesHit = 0;
        levelsBeat = 0;
        Complete = onComplete;
        levelsToBeat = levels;
        SpawnHollowCircles(circlesToSpawn);
    }

    public void RemoveHollowCircle(GameObject hollowCircle)
    {
        if (activeCircles.Contains(hollowCircle))
        {
            activeCircles.Remove(hollowCircle);
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
        if (levelsBeat == levelsToBeat)
        {
            Complete?.Invoke();
        }
        else
        {
            SpawnHollowCircles(circlesToSpawn);
        }
    }

    void SpawnHollowCircles(int numberOfCircles)
    {
        if (baseRingCollider == null)
        {
            Debug.LogError("BaseRingCollider is not assigned!");
            return;
        }

        for (int i = 0; i < numberOfCircles; i++)
        {
            Vector3 spawnPosition = GetRandomPointOnRing();
            int attempts = 0;

            while (IsTooCloseToExistingCircles(spawnPosition) && attempts < 10)
            {
                spawnPosition = GetRandomPointOnRing();
                attempts++;
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

    Vector3 GetRandomPointOnRing()
    {
        if (baseRingCollider == null || baseRingCollider.pathCount == 0)
        {
            Debug.LogError("BaseRingCollider has no defined paths!");
            return Vector3.zero;
        }

        // Get the points of the collider path
        Vector2[] pathPoints = baseRingCollider.points;
        int randomIndex = Random.Range(0, pathPoints.Length);

        // Transform local collider point to world space
        Vector2 worldPosition = baseRingCollider.transform.TransformPoint(pathPoints[randomIndex]);

        return new Vector3(worldPosition.x, worldPosition.y, 0f);
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