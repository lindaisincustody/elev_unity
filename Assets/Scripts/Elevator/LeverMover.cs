using System.Collections;
using UnityEngine;

public class LeverMover : MonoBehaviour
{
    [Header("Lever")] [SerializeField] Transform lever;
    [SerializeField] AnimationCurve leverAnimationCurve;
    [SerializeField] float resetSpeed = 5f; // Speed to reset lever smoothly
    [SerializeField] float rotationSensitivity = 0.2f; // How sensitive the lever is to mouse drag
    [SerializeField] float minRotation = -45f; // Full left (down)
    [SerializeField] float maxRotation = 45f; // Full right (up)
    [SerializeField] float activationThreshold = 30f; // Angle threshold (not used here but kept for reference)

    [Header("Dependencies")] [SerializeField]
    ElevatorManager elevatorManager;

    [SerializeField] ElevatorLevels elevatorLevels;

    private bool isDragging = false;
    private float currentRotation = 0f; // Lever angle (positive for right, negative for left)
    private Vector3 lastMousePosition;
    private bool canRotate = true; // NEW: if false, lever input is ignored

    void Start()
    {
        // Start at neutral (0°)
        currentRotation = 0f;
        lever.rotation = Quaternion.Euler(0, 0, -currentRotation);
    }

    void Update()
    {
        if (!canRotate)
            return; // Do not allow rotation if locked

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            // When the player stops dragging, reset the lever to 0°
            StartCoroutine(ResetLever());
        }

        if (isDragging)
        {
            RotateLeverWithMouse();
            // Calculate a normalized input value (between -1 and +1) based on currentRotation:
            float normalizedInput = currentRotation / maxRotation;
            elevatorLevels.UpdateArrowMovement(normalizedInput);
        }
    }

    private void RotateLeverWithMouse()
    {
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;
        // Positive mouseDelta.x increases currentRotation:
        float rotationAmount = mouseDelta.x * rotationSensitivity;
        currentRotation = Mathf.Clamp(currentRotation + rotationAmount, minRotation, maxRotation);
        // Invert for visual display so that turning right shows right rotation:
        lever.rotation = Quaternion.Euler(0, 0, -currentRotation);
    }

    private IEnumerator ResetLever()
    {
        Quaternion startRotation = lever.rotation;
        Quaternion target = Quaternion.Euler(0, 0, 0f); // Neutral (0°)
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * resetSpeed;
            float curveValue = leverAnimationCurve.Evaluate(t);
            lever.rotation = Quaternion.Lerp(startRotation, target, curveValue);
            yield return null;
        }

        lever.rotation = target;
        currentRotation = 0f;
    }

    // Call this when a mini-game is triggered so the player cannot rotate the lever.
    public void LockLever()
    {
        canRotate = false;
        isDragging = false;
        StartCoroutine(ResetLever());
    }

    // Call this when the mini-game finishes so rotation is allowed again.
    public void UnlockLever()
    {
        canRotate = true;
    }
}