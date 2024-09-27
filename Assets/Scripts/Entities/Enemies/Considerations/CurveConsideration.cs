using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="UtilityAI/Consideration/CurveConsideration")]
public class CurveConsideration : Consideration
{
    public AnimationCurve curve;
    public string contextKey;

    public override float Evaluate(Context context)
    {
        float inputValue = context.GetData<float>(contextKey);

        float utility = curve.Evaluate(inputValue);
        return Mathf.Clamp01(utility);
    }

    void Reset()
    {
        curve = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0));
    }
}
