using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public string abilityId;
    public new string name;
    public float cooldownTime;
    public float activeTime;
    public virtual void Activate() { }
    public virtual void End() { }
}
