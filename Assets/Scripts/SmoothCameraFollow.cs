using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    public float cursorWeight = 0.45f;
    public float maxDistance = 5f;
    public float smoothTime = 0.5f;
    public float smoothTimeFollow = 2f;

    [Header("Right-Click Zoom")]
    public float zoomAmount = 1f;   
    public float zoomSpeed = 3f;

    private Transform player;
    private Camera gameCamera;

    private float defaultOrthoSize;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        player = Player.instance != null ? Player.instance.transform : null;
        gameCamera = Camera.main;
        defaultOrthoSize = gameCamera.orthographicSize;
    }

    /// <summary>
    /// Called by NetworkPlayerSync when the local player spawns over the network,
    /// so the camera switches to follow the correct player on P2's device.
    /// </summary>
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }

    void FixedUpdate()
    {
        // Multiplayer: always follow the LOCAL player's character.
        // NetworkPlayerSync.Local is set in OnNetworkSpawn for the owner, so this
        // self-heals even if SetPlayer() was missed due to a timing race.
        if (NetworkPlayerSync.Local != null)
        {
            if (player != NetworkPlayerSync.Local.transform)
                player = NetworkPlayerSync.Local.transform;
        }
        else if (player == null)
        {
            // Single-player fallback: grab Player.instance when available.
            if (Player.instance != null) player = Player.instance.transform;
            else return;
        }
        // 1) Handle zoom on Right-Click
        float targetSize = Input.GetMouseButton(1)
            ? defaultOrthoSize + zoomAmount
            : defaultOrthoSize;
        gameCamera.orthographicSize = Mathf.Lerp(
            gameCamera.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );

        // 2) Decide target position
        Vector3 desiredPos;
        if (Input.GetMouseButton(1))
        {
            // when holding R-click, center on player
            desiredPos = player.position;

            desiredPos.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPos,
                ref velocity,
                smoothTimeFollow
            );
        }
        else if (TryGetAimScreenPos(out Vector2 aimScreen))
        {
            float depth = gameCamera.WorldToScreenPoint(player.position).z;
            Vector3 mouseWorld = gameCamera.ScreenToWorldPoint(
                new Vector3(aimScreen.x, aimScreen.y, depth));

            desiredPos = Vector3.Lerp(player.position, mouseWorld, cursorWeight);

            Vector3 offset = desiredPos - player.position;
            if (offset.magnitude > maxDistance)
                desiredPos = player.position + offset.normalized * maxDistance;

            desiredPos.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPos,
                ref velocity,
                smoothTime
            );
        }
        else
        {
            // No valid input — smoothly re-centre on the player.
            desiredPos = new Vector3(player.position.x, player.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPos,
                ref velocity,
                smoothTimeFollow
            );
        }

    }

    /// <summary>
    /// Returns a valid screen position on both mouse (editor/desktop) and touch (mobile).
    /// Returns false when no input is present — avoids passing Infinity to ScreenToWorldPoint.
    /// </summary>
    private bool TryGetAimScreenPos(out Vector2 pos)
    {
#if UNITY_EDITOR
        Vector2 mp = Input.mousePosition;
        if (float.IsInfinity(mp.x) || float.IsInfinity(mp.y) ||
            float.IsNaN(mp.x)      || float.IsNaN(mp.y))
        {
            pos = Vector2.zero;
            return false;
        }
        pos = mp;
        return true;
#else
        if (Input.touchCount > 0)
        {
            // Pick the rightmost touch (draw-zone finger).
            Touch best = Input.GetTouch(0);
            for (int i = 1; i < Input.touchCount; i++)
                if (Input.GetTouch(i).position.x > best.position.x)
                    best = Input.GetTouch(i);
            pos = best.position;
            return true;
        }
        pos = Vector2.zero;
        return false;
#endif
    }
}
