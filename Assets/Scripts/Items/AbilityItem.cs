using UnityEngine;

[CreateAssetMenu(fileName = "newItem", menuName = "Custom/Items/AbilityItem")]
public class AbilityItem : Item
{
    public Ability ability;

    public override void Use()
    {
        base.Use();
        Player.instance.Get<PlayerAbilities>().Add(ability);
    }
}
