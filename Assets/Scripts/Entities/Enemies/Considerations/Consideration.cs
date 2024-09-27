using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consideration : ScriptableObject
{
    public abstract float Evaluate(Context context);
}
