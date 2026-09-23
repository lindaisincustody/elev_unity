using UnityEngine;

public class NpcDialogue : DialogueTrigger
{
    [SerializeField] private Npc npc;

    protected override void Trigger()
    {
        npc.Get<NpcBrain>().Pause();
        npc.Get<NpcMovement>().FaceTarget(Player.instance.transform);

        base.Trigger();
    }

    public override void Complete()
    {
        base.Complete();

        npc.Get<NpcBrain>().Resume();
    }

    protected override void Hide()
    {
    }
}
