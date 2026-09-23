using UnityEngine;

public abstract class NpcTrigger : ScriptableObject
{
    public abstract bool IsMet(NpcContext context);
}
