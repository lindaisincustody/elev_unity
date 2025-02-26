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

    // Orthographic zoom settings
    public bool isScoped = false; // Toggle this flag to scope in/out
    public float normalSize = 5f; // Default orthographic size
    public float scopedSize = 2f; // Orthographic size when scoped
    public float zoomSpeed = 5f; // How quickly to zoom in/out

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

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

    void Update()
    {
        // Adjust orthographicSize if using an orthographic camera
        if (cam != null && cam.orthographic)
        {
            float targetSize = isScoped ? scopedSize : normalSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        }
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