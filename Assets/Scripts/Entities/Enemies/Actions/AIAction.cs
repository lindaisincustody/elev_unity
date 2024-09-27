using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : ScriptableObject
{
    public string targetTag;
    public Consideration consideration;

    public virtual void Initialize(Context context)
    {
        
    }

    public float CalculateUtility(Context context) => consideration.Evaluate(context);

    public abstract void Execute(Context context);
    public abstract void Reset(Context context);
}
