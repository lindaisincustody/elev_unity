using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBodyForceApplier : MonoBehaviour
{
    private Transform target;
    public List<Rigidbody2D> jointsrb;

    public void ApplyForce(float forceMagnitude, Vector2 direction)
    {
        Vector2 force = direction.normalized * forceMagnitude;

        foreach (var rb in jointsrb)
        {
            rb.AddForce(force);
        }
    }

    public void MoveToTarget(Transform newTarget, float forceMagnitude, Action onCompleteCallback)
    {
        target = newTarget;
        StartCoroutine(MovePosition(forceMagnitude, onCompleteCallback));
    }

    private IEnumerator MovePosition(float forceMagnitude, Action onCompleteCallback)
    {
        float currentTime = 0f;
        while (currentTime < 5)
        {
            currentTime += Time.deltaTime;
            foreach (var rb in jointsrb)
            {
                Vector2 force = ((Vector2)target.position - rb.position).normalized * forceMagnitude;
                rb.MovePosition(rb.position + force * Time.deltaTime);
                if (Vector2.Distance(target.position, rb.position) < 0.05f)
                {
                    onCompleteCallback?.Invoke();  // Invoke the callback when the action is completed
                    Destroy(gameObject);
                }
            }
            yield return null;
        }
    }
}
