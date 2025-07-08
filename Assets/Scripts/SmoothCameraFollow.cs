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
        player = Player.instance.transform;
        gameCamera = Camera.main;
        defaultOrthoSize = gameCamera.orthographicSize;
    }

    void FixedUpdate()
    {
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
        else
        {
            // your original midpoint logic
            Vector3 mouseScreen = Input.mousePosition;
            mouseScreen.z = gameCamera.WorldToScreenPoint(player.position).z;
            Vector3 mouseWorld = gameCamera.ScreenToWorldPoint(mouseScreen);

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

       
        
    }
}
