using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NpcManager : CoreService
{
    public static NpcManager Instance { get; private set; }

    private readonly List<Npc> npcs = new List<Npc>();
    private readonly List<NpcAnchor> anchors = new List<NpcAnchor>();

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public void RegisterNpc(Npc npc)
    {
        npcs.Add(npc);
    }

    public void UnregisterNpc(Npc npc)
    {
        npcs.Remove(npc);
    }

    public List<Npc> GetAllNpcs()
    {
        return npcs;
    }

    public void RegisterAnchor(NpcAnchor anchor)
    {
        anchors.Add(anchor);
    }

    public void UnregisterAnchor(NpcAnchor anchor)
    {
        anchors.Remove(anchor);
    }

    public Transform GetClosestAnchor(string anchorId, Vector3 position)
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (NpcAnchor anchor in anchors)
        {
            if (anchor.AnchorId != anchorId)
                continue;

            float distance = Vector3.Distance(anchor.transform.position, position);

            if (distance >= closestDistance)
                continue;

            closestDistance = distance;
            closest = anchor.transform;
        }

        return closest;
    }
}
