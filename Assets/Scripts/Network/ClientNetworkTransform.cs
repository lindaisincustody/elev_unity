using Unity.Netcode.Components;
using UnityEngine;

/// <summary>
/// Client-authoritative NetworkTransform — owner sends position to the server
/// so remote proxies can follow, but the owner's transform is NEVER overwritten
/// by NGO's interpolation system.
///
/// Why: Rigidbody2D.MovePosition() moves the physics body every FixedUpdate.
/// Even in client-authoritative mode NGO's NetworkTransform still applies a
/// smoothed/echoed position to the transform on the owner each frame, directly
/// fighting the Rigidbody and making the player appear frozen.
///
/// Fix: disable this component on the owner so it performs no transform writes
/// locally.  Position is synced to remote proxies via SyncedPosition
/// (NetworkVariable<Vector3>) in NetworkPlayerSync instead.
/// </summary>
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative() => false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Disable on ALL instances — owner and non-owner alike.
        //
        // Owner: PlayerMovement.FixedUpdate drives physics; we must not echo
        //        any position back or the Rigidbody freezes.
        //
        // Non-owner: position is driven entirely by NetworkPlayerSync.SyncedPosition
        //            (a NetworkVariable<Vector3> lerped in Update).  Having
        //            NetworkTransform also write to the proxy's Rigidbody creates
        //            two competing rb.MovePosition callers per frame — the result
        //            is the proxy never moving on P2's screen.
        enabled = false;
    }
}
