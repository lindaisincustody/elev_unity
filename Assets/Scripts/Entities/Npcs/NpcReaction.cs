using System;
using UnityEngine;

[Serializable]
public class NpcReaction
{
    public NpcTrigger trigger;
    public NpcTask task;
    public float cooldown = 5f;

    private float nextAllowedTime;

    public bool IsReady(NpcContext context)
    {
        return Time.time >= nextAllowedTime && trigger.IsMet(context);
    }

    public void MarkTriggered()
    {
        nextAllowedTime = Time.time + cooldown;
    }
}
