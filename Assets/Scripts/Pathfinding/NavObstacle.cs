using UnityEngine;

public class NavObstacle : MonoBehaviour
{
    [SerializeField] private Collider2D obstacle;

    private bool wasEnabled;
    private Bounds area;

    private void Start()
    {
        wasEnabled = obstacle.enabled;
        area = obstacle.bounds;
    }

    private void LateUpdate()
    {
        if (obstacle.enabled)
            area = obstacle.bounds;

        if (obstacle.enabled == wasEnabled)
            return;

        wasEnabled = obstacle.enabled;
        NavGridCache.Refresh(area);
    }
}
