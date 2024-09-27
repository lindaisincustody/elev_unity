using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="UtilityAI/Consideration/InRangeConsideration")]
public class InRangeConsideration : Consideration
{
    public float maxDistance = 10f;
    public float maxAngle = 360;
    public string targetTag = "Target";
    public AnimationCurve curve;

    public override float Evaluate(Context context)
    {
        if (!context.sensor.targetTags.Contains(targetTag))
        {
            context.sensor.targetTags.Add(targetTag);
        }

        Transform targetTransform = context.sensor.GetClosestTarget(targetTag);
        if (targetTransform == null) return 0f;
        context.target = targetTransform;
        Transform agentTransform = context.movement.transform;

        float distance = Vector2.Distance(agentTransform.position, targetTransform.position);

        Vector2 directionToTarget = (targetTransform.position - agentTransform.position).normalized;
        float angle = Vector2.Angle(agentTransform.up, directionToTarget);

        bool isInRange = distance <= maxDistance && angle <= maxAngle;

        if (!isInRange) return 0f;

        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        float utility = curve.Evaluate(normalizedDistance);

        return utility;
    }

    void Reset()
    {
        curve = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0));
    }
}
