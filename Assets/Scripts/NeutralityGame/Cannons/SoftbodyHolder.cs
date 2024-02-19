using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftbodyHolder : MonoBehaviour
{
    public List<Transform> softbodies = new List<Transform>();

    public Transform FindClosestSoftbody(Transform targetTransform)
    {
        if (softbodies.Count == 0)
        {
            Debug.LogWarning("No softbodies available in the list.");
            return null;
        }

        Transform closestSoftbody = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform softbody in softbodies)
        {
            float distance = Vector3.Distance(targetTransform.position, softbody.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSoftbody = softbody;
            }
        }

        return closestSoftbody;
    }
}
