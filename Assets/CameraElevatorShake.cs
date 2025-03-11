using System.Collections;
using UnityEngine;

public class CameraElevatorShake : MonoBehaviour
{
    public static CameraElevatorShake instance;
    public float shakeIntensity = 0f; // Set externally for continuous shake.

    private Vector3 basePos; // The camera's current (follow) position without shake

    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        // Each frame, get the camera's current position as set by your follow script.
        basePos = transform.position;

        // If shakeIntensity is non-zero, apply a random offset.
        if (shakeIntensity > 0f)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeIntensity;
            float offsetY = Random.Range(-1f, 1f) * shakeIntensity;
            transform.position = basePos + new Vector3(offsetX, offsetY, 0f);
        }
        // Otherwise, leave the camera at its base position.
    }

    // One-off shake using a coroutine.
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        // Use the basePos captured in LateUpdate as the starting position.
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.position = basePos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // At the end, the LateUpdate will update transform.position based on the follow script.
    }
}