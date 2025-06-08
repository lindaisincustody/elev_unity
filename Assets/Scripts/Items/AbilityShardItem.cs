using UnityEngine;

[CreateAssetMenu(fileName = "newItem", menuName = "Custom/Items/AbilityShardItem")]
public class AbilityShardItem : Item
{
    public AbilityTier Tier;

    public override void Use()
    {
        base.Use();
    }
}
