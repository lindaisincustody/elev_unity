using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UtilityAI/Consideration/CompositeConsideration")]
public class CompositeConsideration : Consideration
{
    public enum OperationType { Average, Multiply, Add, Subtract, Divide, Max, Min}

    public bool allMustBeZero = true;

    public OperationType operation = OperationType.Max;
    public List<Consideration> considerations;

    public override float Evaluate(Context context)
    {
        if (considerations == null || considerations.Count == 0) return 0f;

        float result = considerations[0].Evaluate(context);
        if (result == 0 && allMustBeZero) return 0f;

        for (int i = 1; i < considerations.Count; i++)
        {
            float value = considerations[i].Evaluate(context);

            if (value == 0 && allMustBeZero) return 0f;

            switch (operation)
            {
                case OperationType.Average:
                    result = (result + value) / 2;
                    break;
                case OperationType.Multiply:
                    result *= value;
                    break;
                case OperationType.Add:
                    result += value;
                    break;
                case OperationType.Subtract:
                    result -= value;
                    break;
                case OperationType.Divide:
                    result /= value;
                    break;
                case OperationType.Max:
                    result = Mathf.Max(result, value);
                    break;
                case OperationType.Min:
                    result = Mathf.Min(result, value);
                    break;
            }
        }

        return Mathf.Clamp01(result);
    }

}
