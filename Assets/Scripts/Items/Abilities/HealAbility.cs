using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Heal Ability", menuName = "Custom/Ability/HealAbility")]
public class HealAbility : Ability
{
    [SerializeField] private float duration;
    [SerializeField] private float interval;
    [SerializeField] private int heal;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private HealOvertimeTrigger healPrefab;

    public override void Activate()
    {
        Debug.Log("Heal activated");
        OnActivate?.Invoke();
        HealOvertimeTrigger newHealPrefab = Instantiate(healPrefab, Player.instance.transform.position, Quaternion.identity);
        newHealPrefab.duration = duration;
        newHealPrefab.interval = interval;
        newHealPrefab.heal = heal;
        newHealPrefab.LayerMask = layerMask;
    }

    public override void Destroy()
    {
        Debug.Log("Heal ability on cooldown.");
        OnCooldown?.Invoke();
    }
}
