using UnityEngine;

public class NpcContext
{
    public Npc npc;
    public NpcBrain brain;
    public NpcMovement movement;

    public Transform player => Player.instance.transform;

    public NpcContext(Npc npc, NpcBrain brain, NpcMovement movement)
    {
        this.npc = npc;
        this.brain = brain;
        this.movement = movement;
    }
}
