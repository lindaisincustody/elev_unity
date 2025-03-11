using System.Collections;
using UnityEngine;

public class LeverMover : MonoBehaviour
{
    [Header("Lever")] [SerializeField] Transform lever;
    [SerializeField] AnimationCurve leverAnimationCurve;
    [SerializeField] float resetSpeed = 5f;
    [SerializeField] float rotationSensitivity = 0.2f;
    [SerializeField] float minRotation = -45f;
    [SerializeField] float maxRotation = 45f;
    [SerializeField] float activationThreshold = 30f;

    [Header("Dependencies")] [SerializeField]
    ElevatorManager elevatorManager;

    [SerializeField] ElevatorLevels elevatorLevels;

    private bool isDragging = false;
    private float currentRotation = 0f;
    private Vector3 lastMousePosition;
    private bool canRotate = true;

    void Start()
    {
        currentRotation = 0f;
        lever.rotation = Quaternion.Euler(0, 0, -currentRotation);
    }

    void Update()
    {
        if (!canRotate)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StartCoroutine(ResetLever());
            if (CameraElevatorShake.instance != null)
                CameraElevatorShake.instance.shakeIntensity = 0f;
        }

        if (isDragging)
        {
            RotateLeverWithMouse();
            float normalizedInput = currentRotation / maxRotation;
            elevatorLevels.UpdateArrowMovement(normalizedInput);

            if (CameraElevatorShake.instance != null)
                CameraElevatorShake.instance.shakeIntensity =
                    Mathf.Abs(currentRotation / maxRotation) * 0.05f;
        }
    }

    private void RotateLeverWithMouse()
    {
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;
        float rotationAmount = mouseDelta.x * rotationSensitivity;
        currentRotation = Mathf.Clamp(currentRotation + rotationAmount, minRotation, maxRotation);
        lever.rotation = Quaternion.Euler(0, 0, -currentRotation);
    }

    private IEnumerator ResetLever()
    {
        Quaternion startRotation = lever.rotation;
        Quaternion target = Quaternion.Euler(0, 0, 0f);
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

    public void LockLever()
    {
        canRotate = false;
        isDragging = false;
        StartCoroutine(ResetLever());
        if (CameraElevatorShake.instance != null)
            CameraElevatorShake.instance.shakeIntensity = 0f;
    }

    public void UnlockLever()
    {
        canRotate = true;
    }
}