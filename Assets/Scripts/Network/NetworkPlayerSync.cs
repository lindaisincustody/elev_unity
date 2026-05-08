using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Add this to the Player prefab alongside NetworkObject + NetworkTransform.
///
/// Responsibilities:
///   - Assigns Player.instance to the local owner so all existing single-player
///     code (camera follow, ability registry, etc.) keeps working unchanged.
///   - Disables input and drawing on remote-player proxies so Phone A's touches
///     never drive Phone B's proxy character.
///   - Swaps the Animator controller (and optional sprite tint) for Player 2
///     so both players have distinct looks.
///   - Relays ML drawing results to the other device via ServerRpc → ClientRpc.
///   - Syncs HP via a NetworkVariable so both screens show correct health bars.
/// </summary>
[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayerSync : NetworkBehaviour
{
    // ── Inspector: Player 2 visuals ───────────────────────────────────────

    [Header("Player 2 Visuals")]
    [Tooltip("Animator Controller with Player 2's idle/walk/dash animations. " +
             "Assigned automatically on the remote proxy and on P2's own device.")]
    [SerializeField] private RuntimeAnimatorController playerTwoAnimator;

    [Tooltip("Optional: tint the SpriteRenderer for Player 2 " +
             "(white = no tint). Useful if you share the same sprites but want a colour difference).")]
    [SerializeField] private Color playerTwoColor = Color.white;

    // ── Synced state ──────────────────────────────────────────────────────

    /// <summary>Owner writes, everyone reads.</summary>
    public NetworkVariable<int> SyncedHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    /// <summary>
    /// Movement vector synced every frame from the owner so remote proxies
    /// can drive their Animator without running PlayerMovement locally.
    /// </summary>
    public NetworkVariable<Vector2> SyncedMovement = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    /// <summary>
    /// World-space position written by the owner every FixedUpdate.
    /// Remote proxies lerp to this instead of relying on NetworkTransform,
    /// which was fighting Rigidbody2D.MovePosition() and freezing movement.
    /// </summary>
    public NetworkVariable<Vector3> SyncedPosition = new NetworkVariable<Vector3>(
        Vector3.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    // ── Static convenience accessor ───────────────────────────────────────

    /// <summary>The NetworkPlayerSync that belongs to this device's local player.</summary>
    public static NetworkPlayerSync Local { get; private set; }

    // ── Cached component refs ─────────────────────────────────────────────

    private Player         player;
    private PlayerMovement playerMovement;
    private LetterDrawing  letterDrawing;
    private Animator       proxyAnimator;   // animator on the remote proxy
    private Rigidbody2D    rb;

    // ── Spawn / despawn ───────────────────────────────────────────────────

    public override void OnNetworkSpawn()
    {
        player         = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        rb             = GetComponent<Rigidbody2D>();

        // LetterDrawing may live on the same GO or a child — find it either way.
        letterDrawing = GetComponentInChildren<LetterDrawing>(includeInactive: true);
        if (letterDrawing == null)
            letterDrawing = FindObjectOfType<LetterDrawing>(); // scene-level fallback

        // ── Determine player slot ─────────────────────────────────────────
        // Host always has ServerClientId (0); the joining client gets a higher ID.
        // We use this to decide visuals — no extra NetworkVariable needed because
        // OwnerClientId is automatically synchronised by NGO.
        bool isPlayerTwo = OwnerClientId != NetworkManager.ServerClientId;
        if (isPlayerTwo)
            ApplyPlayerTwoVisuals();

        // ── Diagnostic snapshot ───────────────────────────────────────────
        // Logs appear on BOTH devices so you can compare them side by side.
        var inputMgrDiag = GetComponent<InputManager>();
        var pmDiag       = playerMovement;
        var ldDiag       = letterDrawing;
        Debug.Log($"[Net] OnNetworkSpawn  GO={name}  IsOwner={IsOwner}  OwnerClientId={OwnerClientId}" +
                  $"  LocalClientId={NetworkManager.Singleton.LocalClientId}" +
                  $"  HasInputMgr={inputMgrDiag != null}  HasPM={pmDiag != null}" +
                  $"  PM.enabled={pmDiag?.enabled}  HasLD={ldDiag != null}" +
                  $"  LD.enabled={ldDiag?.enabled}");

        // ── Local vs remote setup ─────────────────────────────────────────
        if (IsOwner)
        {
            // LOCAL player: point the singleton so all existing code works.
            Local = this;
            Player.instance = player;

            // Camera scripts cache Player.instance.transform in their Start/Awake,
            // which runs before network objects spawn. Re-point them now so P2's
            // camera follows their own character instead of P1's proxy.
            var smoothCam = FindObjectOfType<SmoothCameraFollow>();
            if (smoothCam != null) smoothCam.SetPlayer(transform);

            var cineCam = FindObjectOfType<CinemachineTopDownAim_CM2>();
            if (cineCam != null) cineCam.SetPlayer(transform);

            // Wire the scene joystick to this player's InputManager so P2 can move.
            // P1 is already wired via the Inspector; calling SetJoystick on P1 is harmless.
            var joystick = FindObjectOfType<TouchJoystick>(true); // include inactive
            var inputMgr = GetComponent<InputManager>();
            if (joystick != null && inputMgr != null)
                inputMgr.SetJoystick(joystick);

            Debug.Log($"[Net] Owner setup — joystick={(joystick != null ? joystick.name : "NULL")}" +
                      $"  inputMgr={(inputMgr != null ? "found" : "NULL — InputManager missing from prefab!")}");

            // Wire LetterDrawing scene references that can't survive in a prefab
            // (camera, UI text, etc.). Copy them from P1's scene-placed LetterDrawing
            // which has everything set up via the Inspector.
            if (letterDrawing != null)
                WireLetterDrawingReferences();

            // Re-point the scene DrawingZoneController to THIS player's LetterDrawing
            // so the grid-reveal animation and finger-UV pulse react to the local
            // player's drawing state (not P1's scene-placed reference).
            if (letterDrawing != null)
            {
                var dzc = FindObjectOfType<DrawingZoneController>(true);
                if (dzc != null)
                    dzc.SetLetterDrawing(letterDrawing);
                else
                    Debug.LogWarning("[Net] DrawingZoneController not found in scene — grid reveal won't react to P2 drawing.");
            }

            // Re-enable EnhancedTouchSupport unconditionally for the local owner.
            // Disabling remote-proxy LetterDrawings (below) calls OnDisable() which
            // calls EnhancedTouchSupport.Disable(). If that races with (or happens
            // after) the local LetterDrawing's Enable(), touch input goes silent.
            // Calling Enable() here guarantees touch is on for the local player
            // regardless of ordering.
            EnhancedTouchSupport.Enable();

            Debug.Log($"[Net] Local player spawned (slot: {(isPlayerTwo ? "P2" : "P1")}).");
        }
        else
        {
            // REMOTE proxy: disable input so this device can't drive the other player.
            if (playerMovement != null)
                playerMovement.enabled = false;

            // Make the proxy Rigidbody kinematic so physics forces (gravity, collisions
            // with the local player) can't push it away from the SyncedPosition lerp
            // target.  NetworkRigidbody2D used to do this automatically — we do it
            // manually now that that component has been removed from the prefab.
            if (rb != null)
                rb.isKinematic = true;

            // Disable drawing only if LetterDrawing lives on the Player GO itself.
            // If it's a separate scene object it already belongs to the local player.
            if (letterDrawing != null && letterDrawing.gameObject == player.gameObject)
                letterDrawing.enabled = false;

            // Safety net: if disabling the proxy's LetterDrawing somehow triggered
            // EnhancedTouchSupport.Disable() (e.g. residual code paths), restore it
            // for the local player if they are already spawned.
            if (NetworkPlayerSync.Local != null)
                EnhancedTouchSupport.Enable();

            // Cache the animator so we can drive it from SyncedMovement changes.
            proxyAnimator = GetComponent<Animator>();
            if (proxyAnimator == null) proxyAnimator = GetComponentInChildren<Animator>();
            SyncedMovement.OnValueChanged += OnSyncedMovementChanged;

            Debug.Log($"[Net] Remote proxy spawned (slot: {(isPlayerTwo ? "P2" : "P1")}).");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            Local = null;
            EnhancedTouchSupport.Disable(); // balance the Enable() from OnNetworkSpawn
        }
        else SyncedMovement.OnValueChanged -= OnSyncedMovementChanged;
    }

    // ── Position + animation sync ─────────────────────────────────────────

    private void FixedUpdate()
    {
        if (!IsOwner || rb == null) return;

        // Owner: publish our Rigidbody's current physics position so remote
        // proxies can follow.  We read rb.position (physics) not transform.position
        // because between Update and the next physics step those can differ.
        // Only write when we've actually moved to save bandwidth.
        Vector3 pos = rb.position;
        if ((SyncedPosition.Value - pos).sqrMagnitude > 0.0001f)
            SyncedPosition.Value = pos;
    }

    private void Update()
    {
        // ── Non-owner: apply synced position to this proxy ────────────────
        if (!IsOwner)
        {
            if (rb != null)
            {
                // Lerp the Rigidbody to the received position so the proxy moves
                // smoothly rather than teleporting.  15f gives snappy but not
                // instant catch-up; raise it if proxies feel laggy.
                rb.MovePosition(Vector2.Lerp(rb.position, SyncedPosition.Value, 15f * Time.deltaTime));
            }
            return;
        }

        if (playerMovement == null) return;

        // Owner: write movement every frame so remote proxies can animate.
        // Only write when the value actually changes to save bandwidth.
        Vector2 move = playerMovement.movement;
        if (SyncedMovement.Value != move)
            SyncedMovement.Value = move;
    }

    /// <summary>Fires on every device that has a remote proxy for this player.</summary>
    private void OnSyncedMovementChanged(Vector2 previous, Vector2 current)
    {
        if (proxyAnimator == null) return;
        proxyAnimator.SetFloat("Horizontal", current.x);
        proxyAnimator.SetFloat("Vertical",   current.y);
        proxyAnimator.SetFloat("Speed",       current.sqrMagnitude);
    }

    // ── Visuals ───────────────────────────────────────────────────────────

    /// <summary>
    /// Swaps the Animator controller and optionally tints the sprite for Player 2.
    /// Runs on BOTH devices for the P2 object, so every screen shows the right look.
    /// </summary>
    private void ApplyPlayerTwoVisuals()
    {
        // ── Animator controller ───────────────────────────────────────────
        if (playerTwoAnimator != null)
        {
            var anim = GetComponent<Animator>();
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.runtimeAnimatorController = playerTwoAnimator;
                Debug.Log($"[Net] P2 animator applied: {playerTwoAnimator.name}");
            }
            else
                Debug.LogWarning("[Net] P2 animator NOT applied — no Animator component found on player or children.");
        }
        else
            Debug.LogWarning("[Net] P2 animator NOT applied — playerTwoAnimator is not assigned in the Inspector on the Player prefab's NetworkPlayerSync.");

        // ── Sprite tint (optional) ────────────────────────────────────────
        if (playerTwoColor != Color.white)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.color = playerTwoColor;
        }
    }

    // ── Drawing result sync ───────────────────────────────────────────────

    /// <summary>
    /// Called by PredictingDrawingState after ML produces a result.
    /// Sends the raw label (for AbilityActionRegistry) and the pre-converted
    /// Unicode glyph (for the visual symbol pop) to all other clients.
    /// </summary>
    public void BroadcastDrawingResult(string label, string glyph)
    {
        if (!IsOwner) return;
        DrawingResultServerRpc(label, glyph);
    }

    [ServerRpc]
    private void DrawingResultServerRpc(string label, string glyph)
    {
        DrawingResultClientRpc(label, glyph);
    }

    [ClientRpc]
    private void DrawingResultClientRpc(string label, string glyph)
    {
        // Owner already applied the result locally — skip double-processing.
        if (IsOwner) return;

        // Trigger the remote player's ability and enemy checks on this device.
        AbilityActionRegistry.Instance?.ExecuteAction(label);

        // Spawn the glyph symbol above this player's proxy so the other screen
        // sees the same symbol pop that the drawing player sees locally.
        SpawnSymbolOnProxy(glyph);

        Debug.Log($"[Net] Remote player drew: {label} ({glyph})");
    }

    /// <summary>
    /// Instantiates the symbol prefab above THIS object's transform (P1's proxy on P2's device).
    /// Asset refs (symbolPrefab, symbolScale, etc.) are taken from the LOCAL player's
    /// LetterDrawing, which has everything wired via the prefab and WireLetterDrawingReferences.
    /// </summary>
    private void SpawnSymbolOnProxy(string glyph)
    {
        var ld = NetworkPlayerSync.Local?.letterDrawing;
        if (ld == null)
        {
            Debug.LogWarning("[Net] SpawnSymbolOnProxy: no local LetterDrawing found.");
            return;
        }
        if (ld.symbolPrefab == null)
        {
            Debug.LogWarning("[Net] SpawnSymbolOnProxy: symbolPrefab is null on local LetterDrawing.");
            return;
        }

        // Spawn the TMP glyph parented to the remote proxy so it tracks its movement.
        var tmp = Instantiate(ld.symbolPrefab, transform.position, Quaternion.identity);
        tmp.text = glyph;
        tmp.transform.localScale = Vector3.zero;

        tmp.transform.SetParent(transform, worldPositionStays: false);
        tmp.transform.localPosition = Vector3.up * ld.symbolVerticalOffset;

        Camera refCam = ld.gameplayCamera != null ? ld.gameplayCamera : Camera.main;
        if (refCam != null) tmp.transform.rotation = refCam.transform.rotation;

        // Counter-act the proxy's lossy scale so the glyph renders at exactly symbolScale
        // world units regardless of the player sprite's non-uniform scale.
        float worldSize = ld.symbolScale;
        Vector3 ls = transform.lossyScale;
        float sx = Mathf.Abs(ls.x) > 1e-4f ? worldSize / ls.x : worldSize;
        float sy = Mathf.Abs(ls.y) > 1e-4f ? worldSize / ls.y : worldSize;
        float sz = Mathf.Abs(ls.z) > 1e-4f ? worldSize / ls.z : worldSize;
        Vector3 targetScale = new Vector3(sx, sy, sz);

        var tmpMesh = tmp.GetComponent<TextMeshPro>();
        StartCoroutine(StampSymbol(tmpMesh.transform, tmpMesh, targetScale, ld.symbolStampDuration));
        Destroy(tmp.gameObject, ld.symbolLifetime);
    }

    /// <summary>Back-out eased stamp animation — mirrors PredictingDrawingState.ComplexStamp.</summary>
    private IEnumerator StampSymbol(Transform sym, TextMeshPro tmp, Vector3 targetScale, float duration)
    {
        float BackOut(float t) { const float s = 1.70158f; t -= 1f; return t * t * ((s + 1f) * t + s) + 1f; }

        float elapsed = 0f;
        Color baseColor = tmp.color;
        tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        sym.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            sym.localScale    = Vector3.one * (targetScale.x * BackOut(t));
            sym.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * Mathf.PI) * (1f - t) * 2.5f);
            tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b,
                                  Mathf.Clamp01(t / 0.3f) * baseColor.a);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sym.localScale    = targetScale;
        sym.localRotation = Quaternion.identity;
        tmp.color         = baseColor;
    }

    // ── Health sync ───────────────────────────────────────────────────────

    /// <summary>Call this whenever the local player's HP changes.</summary>
    public void SyncHealth(int newHp)
    {
        if (!IsOwner) return;
        SyncedHealth.Value = newHp;
    }

    // ── Drawing wiring ────────────────────────────────────────────────────

    /// <summary>
    /// Copies scene-specific references (camera) from the scene-placed P1 LetterDrawing
    /// to this dynamically spawned player's LetterDrawing.
    ///
    /// IMPORTANT: drawingDisplay / renderTextureDisplay are SCENE objects (inside
    /// JoystickZone), not prefab children.  They are null on the prefab clone and
    /// must be copied from the source.  After copying, RefreshDrawingDisplayTexture()
    /// is called so the display camera's RenderTexture is re-wired to the RawImage
    /// (PredictingDrawingState set it in Start() but drawingDisplay was null then).
    ///
    /// currentLetterText / poemTextDisplay may be prefab children OR scene objects
    /// depending on the project setup — null-guard handles both cases.
    ///
    /// Only truly scene-level refs that can't live in a prefab (the gameplay camera)
    /// need to be copied here.  Everything else is null-guarded.
    /// </summary>
    private void WireLetterDrawingReferences()
    {
        // Find any other LetterDrawing in the scene that already has its camera wired.
        // That will be P1's scene-placed player — it has the gameplay camera set in the Inspector.
        LetterDrawing source = null;
        foreach (var ld in FindObjectsOfType<LetterDrawing>(true))
        {
            if (ld != letterDrawing && ld.gameplayCamera != null)
            {
                source = ld;
                break;
            }
        }

        if (source != null)
        {
            // ── Scene-level camera (MUST be copied — prefab can't hold a scene ref) ──
            letterDrawing.gameplayCamera = source.gameplayCamera;

            // ── Scene UI (drawingDisplay / renderTextureDisplay are scene objects) ─────
            // Always copy these — the prefab clone has them null.
            // After assigning drawingDisplay, RefreshDrawingDisplayTexture() re-wires
            // the RenderTexture that PredictingDrawingState created before this ran.
            if (letterDrawing.drawingDisplay == null)
                letterDrawing.drawingDisplay = source.drawingDisplay;
            if (letterDrawing.renderTextureDisplay == null)
                letterDrawing.renderTextureDisplay = source.renderTextureDisplay;

            // Re-wire the display RenderTexture NOW (InitializeCamera ran in Start()
            // when drawingDisplay was still null, so the texture was never assigned).
            letterDrawing.RefreshDrawingDisplayTexture();

            // ── Other per-player UI (may be prefab children; null-guard) ─────────────
            if (letterDrawing.currentLetterText == null)
                letterDrawing.currentLetterText = source.currentLetterText;
            if (letterDrawing.poemTextDisplay == null)
                letterDrawing.poemTextDisplay = source.poemTextDisplay;

            // ── Stroke tuning (always mirror P1's Inspector values) ───────────────────
            // drawStrokeWidth is serialised on the prefab but its default may differ
            // from the scene-placed P1 LetterDrawing if P1 was tuned in the Inspector.
            // Copying it here means you only need to set it once (on the scene Player).
            letterDrawing.drawStrokeWidth = source.drawStrokeWidth;

            // ── Asset references (null-guarded safety net) ────────────────────────────
            if (letterDrawing.symbolPrefab == null)
                letterDrawing.symbolPrefab          = source.symbolPrefab;
            if (letterDrawing.primaryLineMaterial == null)
                letterDrawing.primaryLineMaterial   = source.primaryLineMaterial;
            if (letterDrawing.groundMaterial == null)
                letterDrawing.groundMaterial        = source.groundMaterial;
            if (letterDrawing.trippyTransparentMaterial == null)
                letterDrawing.trippyTransparentMaterial = source.trippyTransparentMaterial;
            if (letterDrawing.model == null)
                letterDrawing.model                 = source.model;

            Debug.Log($"[Net] P2 LetterDrawing references wired from scene." +
                      $"  gameplayCam={(letterDrawing.gameplayCamera != null ? letterDrawing.gameplayCamera.name : "NULL")}" +
                      $"  drawingDisplay={(letterDrawing.drawingDisplay != null ? "OK" : "NULL!")}" +
                      $"  displayTex={(letterDrawing.drawingDisplay?.texture != null ? letterDrawing.drawingDisplay.texture.name : "NULL — RT not assigned!")}" +
                      $"  model={(letterDrawing.model != null ? letterDrawing.model.name : "NULL")}" +
                      $"  primaryMat={(letterDrawing.primaryLineMaterial != null ? "OK" : "NULL")}" +
                      $"  secondaryMat={(letterDrawing.trippyTransparentMaterial != null ? "OK" : "NULL")}");
        }
        else
        {
            // Graceful fallback — at minimum assign Camera.main so drawing doesn't crash.
            if (letterDrawing.gameplayCamera == null)
                letterDrawing.gameplayCamera = Camera.main;
            Debug.LogWarning("[Net] WireLetterDrawingReferences: no source LetterDrawing found; using Camera.main fallback.");
        }

        // playerTransform must always point to THIS player's own transform.
        letterDrawing.playerTransform = transform;
    }
}
