using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // make sure you have TextMeshPro imported

[CreateAssetMenu(fileName = "DiagupAbility", menuName = "Custom/Ability/DiagupAbility")]
public class DiagupAbility : Ability
{
    [Tooltip("How many slashes before they fire")]
    public int threshold = 3;

    [Tooltip("Speed at which each slash flies")]
    public float slashMoveSpeed = 10f;

    [Tooltip("Damage per slash")]
    public int slashDamage = 1;

    // Holds all active slashes on screen
    private readonly List<SlashSymbolController> activeSlashes = new List<SlashSymbolController>();
    private Transform symbolParent;

    public override void Activate()
    {
        OnActivate?.Invoke();

        // Create a parent object for cleanliness, once
        if (symbolParent == null)
        {
            var go = new GameObject("SlashSymbols");
            symbolParent = go.transform;
        }
    }

    public override void Destroy()
    {
        OnCooldown?.Invoke();
    }

    /// <summary>
    /// Call this whenever you detect a "/" draw. It will spawn a TextMeshPro "/" in world space.
    /// </summary>
    public void SpawnSlashAt(Vector2 worldPos)
    {
        // 1) Create a new GameObject at runtime
        var go = new GameObject("SlashSymbol");
        go.transform.position = worldPos;
        go.transform.SetParent(symbolParent);

        // 2) Add and configure a TextMeshPro component
        var tm = go.AddComponent<TextMeshPro>();
        tm.text = "/";
        tm.fontSize = 5;               // adjust size to taste
        tm.alignment = TextAlignmentOptions.Center;
        tm.color = Color.white;

        // 3) Add a Rigidbody2D so triggers work
        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 4) Add a trigger Collider2D
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;             // tweak if needed

        // 5) Add your SlashSymbolController and configure it
        var ctrl = go.AddComponent<SlashSymbolController>();
        ctrl.moveSpeed = slashMoveSpeed;
        ctrl.damage = slashDamage;
        ctrl.OnTackleComplete += OnSlashTackleComplete;

        // 6) Track it
        activeSlashes.Add(ctrl);

        // 7) Once we hit the threshold, send them at the nearest enemy
        if (activeSlashes.Count >= threshold)
            TriggerTackle();
    }

    private void OnSlashTackleComplete(SlashSymbolController slash)
    {
        activeSlashes.Remove(slash);
    }

    private void TriggerTackle()
    {
        // 1) Use your PlayerCombat helper instead of FindWithTag
        Enemy targetEnemy = Player.instance.Get<PlayerCombat>().GetNearestEnemy();
        if (targetEnemy == null) return;

        // 2) Tell every slash to tackle that Enemy
        foreach (var slash in activeSlashes)
        {
            slash.BeginTackle(targetEnemy);
        }

        // 3) Reset for the next batch
        activeSlashes.Clear();
    }
}