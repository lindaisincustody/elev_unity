using UnityEngine;

public class Npc : Entity
{
    private void Awake()
    {
        foreach (Component component in components)
        {
            component.Init(this);
        }
    }

    private void OnDestroy()
    {
        NpcManager.Instance.UnregisterNpc(this);
    }
}
