using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float damping;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        Vector3 movePosition = target.position + offset;
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(movePosition.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(movePosition.y, minBounds.y, maxBounds.y),
            movePosition.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, clampedPosition, ref velocity, damping);
    }

    public Vector2 GetMinBounds()
    {
        return minBounds;
    }

    public Vector2 GetMaxBounds()
    {
        return maxBounds;
    }

    public void SetMinBounds(Vector2 newMinBounds)
    {
        minBounds = newMinBounds;
    }

    public void SetMaxBounds(Vector2 newMaxBounds)
    {
        maxBounds = newMaxBounds;
    }
}
