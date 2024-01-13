using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBodyForceApplier : MonoBehaviour
{
    public List<Rigidbody2D> jointsrb;

    public void ApplyForce(float forceMagnitude, Vector2 direction)
    {
        Vector2 force = direction.normalized * forceMagnitude;

        foreach (var rb in jointsrb)
        {
            rb.AddForce(force);
        }
    }
}
