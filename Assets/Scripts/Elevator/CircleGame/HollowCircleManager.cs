using System.Collections.Generic;
using UnityEngine;

public class HollowCircleManager : MonoBehaviour
{
    [SerializeField] private HollowCircle hollowCirclePrefab;
    [SerializeField] private RectTransform spawnParent;
    [SerializeField] private float spawnRadius = 365f;

    private int circlesToSpawn = 1;
    private int circlesHit = 0;
    private int levelsToBeat = 3;
    private int levelsBeat = 0;
    private readonly List<HollowCircle> activeCircles = new List<HollowCircle>();
    private Animator animator;
    private System.Action Complete;

    private RectTransform Ring => (RectTransform)transform;

    private void Awake()
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

    public HollowCircle FindCircleAtAngle(float angle, float tolerance)
    {
        foreach (HollowCircle circle in activeCircles)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(angle, circle.Angle)) <= tolerance)
                return circle;
        }

        return null;
    }

    public void RemoveHollowCircle(HollowCircle hollowCircle)
    {
        if (!activeCircles.Remove(hollowCircle))
            return;

        Destroy(hollowCircle.gameObject);
        circlesHit++;

        if (circlesHit == circlesToSpawn)
            ProgressToNextLevel();
    }

    public void ClearCircles()
    {
        foreach (HollowCircle circle in activeCircles)
            Destroy(circle.gameObject);

        activeCircles.Clear();
        circlesHit = 0;
    }

    public void ResetGameToLevel1()
    {
        circlesToSpawn = 1;
        levelsBeat = 0;
        ClearCircles();
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

    private void ProgressToNextLevel()
    {
        circlesHit = 0;
        circlesToSpawn++;
        levelsBeat++;

        if (levelsBeat == levelsToBeat)
            Complete?.Invoke();
        else
            SpawnHollowCircles(circlesToSpawn);
    }

    private void SpawnHollowCircles(int numberOfCircles)
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            float angle = (360f / numberOfCircles) * i;

            HollowCircle circle = Instantiate(hollowCirclePrefab, spawnParent);
            circle.Rect.anchoredPosition = Ring.anchoredPosition + AngleToOffset(angle);
            circle.Initialize(this, angle);

            activeCircles.Add(circle);
        }
    }

    private Vector2 AngleToOffset(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * spawnRadius;
    }
}
