using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowCircleManager : MonoBehaviour
{
    public GameObject hollowCirclePrefab;
    public PolygonCollider2D baseRingCollider;

    private int circlesToSpawn = 1;
    private int circlesHit = 0;
    private int levelsToBeat = 3;
    private int levelsBeat = 0;
    private List<GameObject> activeCircles = new List<GameObject>();
    private float minDistanceBetweenCircles = 1.5f;
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

        Vector2 ringCenter = baseRingCollider.transform.TransformPoint(baseRingCollider.offset);
        Vector2 firstPoint = baseRingCollider.points[0] + baseRingCollider.offset;
        Vector2 worldFirstPoint = baseRingCollider.transform.TransformPoint(firstPoint);
        float ringRadius = Vector2.Distance(ringCenter, worldFirstPoint);

        for (int i = 0; i < numberOfCircles; i++)
        {
            float angle = (2 * Mathf.PI / numberOfCircles) * i;
            Vector2 spawnPosition = ringCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ringRadius;
            GameObject circle = Instantiate(hollowCirclePrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0f),
                Quaternion.identity);
            HollowCircle circleScript = circle.GetComponent<HollowCircle>();

            if (circleScript != null)
            {
                circleScript.Initialize(this);
                activeCircles.Add(circle);
            }
        }
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