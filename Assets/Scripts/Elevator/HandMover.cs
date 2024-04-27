using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMover : MonoBehaviour
{
    [SerializeField] Transform pointToMove;
    [SerializeField] Vector3 idlePos;
    [SerializeField] float moveSpeed = 5f;

    private Transform target;

    private bool follow = false;

    public System.Action OnHandReset;

    public void SetActionOnHandReset(System.Action onReset)
    {
        OnHandReset = onReset;
    }

    public void MoveHand(Transform newTarget, System.Action OnCompelte)
    {
        target = newTarget;
        follow = true;
        StartCoroutine(MoveHandOverTime(newTarget.position, OnCompelte));
    }

    public void MoveHandBack()
    {
        StartCoroutine(MoveHandOverTime(idlePos, ResetHand));
    }

    private void ResetHand()
    {
        follow = false;
        target = null;
        OnHandReset?.Invoke();
    }

    private IEnumerator MoveHandOverTime(Vector3 moveTo, System.Action OnCompelte)
    {
        Vector3 startPos = pointToMove.position;
        Vector3 endPos = moveTo;

        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            pointToMove.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            yield return null;
        }

        pointToMove.position = endPos;
        OnCompelte?.Invoke();
    }

    private void Update()
    {
        if (follow)
        {
            pointToMove.position = Vector3.MoveTowards(pointToMove.position, target.position, moveSpeed * Time.deltaTime);
        }
    }
}
