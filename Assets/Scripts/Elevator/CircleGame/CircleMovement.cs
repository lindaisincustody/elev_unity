using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    [Header("References")] public PolygonCollider2D baseRingCollider;
    [SerializeField] private HollowCircleManager mngr;

    [Header("Movement Settings")] public float rotationSpeed = 1f;

    public Vector2 playerColliderOffset = new Vector2(-3.6f, 0f);

    public float ringThickness = 1f;

    private bool isPinAligned = false;
    public bool isActive = false;
    private HollowCircle collidedHollowCircle;

    private Vector2 ringCenter;
    private float ringRadius;
    private float effectiveRadius;
    private float currentAngle;

    void Start()
    {
        if (baseRingCollider == null)
        {
            Debug.LogError("BaseRingCollider is not assigned in CircleMovement!");
            return;
        }

        ringCenter = baseRingCollider.transform.TransformPoint(baseRingCollider.offset);

        Vector2 firstPoint = baseRingCollider.points[0] + baseRingCollider.offset;
        Vector2 worldFirstPoint = baseRingCollider.transform.TransformPoint(firstPoint);
        ringRadius = Vector2.Distance(ringCenter, worldFirstPoint);

        effectiveRadius = ringRadius - ringThickness / 2f;

        currentAngle = 0f;

        Vector2 newPos = ringCenter + new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * effectiveRadius;
        transform.position = new Vector3(newPos.x - playerColliderOffset.x, newPos.y - playerColliderOffset.y,
            transform.position.z);
    }

    void Update()
    {
        if (!isActive)
            return;

        currentAngle += rotationSpeed * Time.deltaTime;
        Vector2 newPos = ringCenter + new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * effectiveRadius;
        transform.position = new Vector3(newPos.x - playerColliderOffset.x, newPos.y - playerColliderOffset.y,
            transform.position.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (HasCollidedWithHollowCircle())
            {
                HitHollowCircle();
            }
            else
            {
                mngr.MissAnimation();
            }
        }
    }

    private bool HasCollidedWithHollowCircle()
    {
        return isPinAligned && collidedHollowCircle != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HollowCircle"))
        {
            isPinAligned = true;
            collidedHollowCircle = other.GetComponent<HollowCircle>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HollowCircle"))
        {
            isPinAligned = false;
            collidedHollowCircle = null;
        }
    }

    void HitHollowCircle()
    {
        if (collidedHollowCircle != null)
        {
            collidedHollowCircle.HitHollowCircle();
        }
    }
}