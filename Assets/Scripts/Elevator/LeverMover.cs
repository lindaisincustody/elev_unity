using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverMover : MonoBehaviour
{
    [Header("Lever")]
    [SerializeField] Transform lever;
    [SerializeField] HandMover hand;
    [SerializeField] Transform handle;
    [SerializeField] AnimationCurve leverAnimationCurve;
    [SerializeField] float rotationSpeed = 0.5f;
    private float maxRotation = -45f;

    [Header("Shifter")]
    [SerializeField] Transform shifter;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] float pullSpeed = 0.5f;
    [SerializeField] float yPos = -1;

    private System.Action shift;

    public void ShiftLeft(System.Action onShift)
    {
        shift = onShift;
        hand.MoveHand(handle, () => StartCoroutine(RotateLever(-maxRotation, ResetHand)));
    }

    public void ShiftRight(System.Action onShift)
    {
        shift = onShift;
        hand.MoveHand(handle, () => StartCoroutine(RotateLever(maxRotation, ShiftBack)));
    }

    private void ShiftBack()
    {
        shift?.Invoke();
        StartCoroutine(RotateLever(0, ResetHand));
    }

    public void PullLever()
    {
        hand.MoveHand(shifter, () => StartCoroutine(ShiftLever(yPos, animationCurve, ResetHand)));
    }

    public void ResetLever()
    {
        hand.MoveHand(handle, () => StartCoroutine(RotateLever(0f, ResetHand)));
    }

    IEnumerator ShiftLever(float targetPosition, AnimationCurve curve, System.Action OnComplete)
    {
        Vector3 startPosition = shifter.position;
        Vector3 target = new Vector3(startPosition.x, targetPosition, startPosition.z);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * pullSpeed;
            float curveValue = curve.Evaluate(t);
            shifter.position = Vector3.Lerp(startPosition, target, curveValue);
            yield return null;
        }

        // Ensure the object reaches the target position precisely
        shifter.position = target;

        // Reverse motion
        Vector3 reverseStartPosition = target;
        Vector3 reverseTargetPosition = startPosition;

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * pullSpeed;
            float curveValue = curve.Evaluate(t);
            shifter.position = Vector3.Lerp(reverseStartPosition, reverseTargetPosition, curveValue);
            yield return null;
        }

        // Ensure the object reaches the start position precisely
        shifter.position = startPosition;

        // Call the onComplete action
        OnComplete?.Invoke();
    }

    IEnumerator RotateLever(float targetRotation, System.Action OnComplete)
    {
        Quaternion startRotation = lever.rotation;
        Quaternion target = Quaternion.Euler(0, 0, targetRotation);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * rotationSpeed;
            float curveValue = leverAnimationCurve.Evaluate(t);
            lever.rotation = Quaternion.Lerp(startRotation, target, curveValue);
            yield return null;
        }

        // Ensure the object reaches the target rotation precisely
        lever.rotation = target;

        // Call the onComplete action
        OnComplete?.Invoke();
    }

    private void ResetHand()
    {
        hand.MoveHandBack();
    }
}
