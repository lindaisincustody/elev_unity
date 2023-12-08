using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquare : MonoBehaviour
{
    private float movementSpeed = 2f;
    private bool isNearCharacter = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform playerTransform;

    private float maxDistanceFromOriginal = 0.1f;
    private float maxRotationAngle = 10f; // Maximum rotation angle in degrees
    private float pushRadius = 1.5f;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        playerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (!isNearCharacter)
            return;

        float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceFromPlayer < pushRadius)
            MoveAwayFromPlayer();
    }

    private void MoveAwayFromPlayer()
    {
        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        Vector3 newPosition = transform.position + directionAwayFromPlayer * movementSpeed * Time.deltaTime;

        // Calculate target rotation
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionAwayFromPlayer);

        if (Vector3.Distance(newPosition, originalPosition) <= maxDistanceFromOriginal)
        {
            transform.position = newPosition;
        }
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        isNearCharacter = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isNearCharacter = false;
        StartCoroutine(ResetState());
    }

    private IEnumerator ResetState()
    {
        float duration = 2f; // Duration of the reset state in seconds
        float elapsed = 0f; // Timer to track elapsed time

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, movementSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, originalPosition, movementSpeed * Time.deltaTime);
            elapsed += Time.deltaTime; // Increment the timer
            yield return null; // Wait for the next frame
        }

        // Ensure the object is exactly at the original position and rotation after 2 seconds
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
