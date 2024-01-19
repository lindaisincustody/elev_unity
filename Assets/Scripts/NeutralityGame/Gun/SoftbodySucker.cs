using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftbodySucker
{
    private Color color;
    public void SuckIn(SoftBodyForceApplier softbody, Transform target, float power, SpriteRenderer liquidRenderer, Action onCompleteCallback)
    {
        color = softbody.GetComponent<SpriteRenderer>().color;
        liquidRenderer.color = color;
        softbody.MoveToTarget(target, power, onCompleteCallback);
    }

    public void ShootSoftBody(SoftBodyForceApplier softBody, float power, Vector2 direction)
    {
        softBody.ApplyForce(power, direction);
    }

    public Color GetColor()
    {
        return color;
    }
}
