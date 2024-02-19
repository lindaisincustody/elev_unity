using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftbodyFollower : MonoBehaviour
{
    private Transform target;
    private Vector3 offscreenPos = new Vector3(500, 500, 1);

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.position = target.position;
        else
            transform.position = offscreenPos;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
