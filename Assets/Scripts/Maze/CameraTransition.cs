using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public Transform playerTransform;
    public int maxCameraFOV = 80;
    public int targetCameraFov = 5;
    public float transitionDuration = 2.0f;
    public AnimationCurve transitionCurve;

    private Camera mainCamera;
    private Vector3 initialCameraPosition;
    float newFOVSize;
    [SerializeField] private MazePlayerMovement player;
    [SerializeField] private SpriteRenderer fogOfWar;

    public float fogOfWarAlpha;

    public bool canMove = false;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (canMove)
            mainCamera.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -20);
    }

    public void SetCamera(int mazeWidth, int mazeHeight, int cellSize)
    {
        initialCameraPosition = new Vector3(mazeWidth * cellSize / 2 - 5, mazeHeight * cellSize / 2 - 5, -20);
        newFOVSize = (Mathf.Max(mazeHeight, mazeWidth) / 30.0f) * maxCameraFOV;
        mainCamera.transform.position = initialCameraPosition;
        mainCamera.orthographicSize = newFOVSize;
    }

    public void StartZoomIn()
    {
        StartCoroutine(ZoomIn());
    }

    private IEnumerator ZoomIn()
    {
        float startTime = Time.time;
        float progress = 0;
        float initialFogAlpha = fogOfWar.color.a;

        while (progress < 1)
        {
            progress = Mathf.Clamp01((Time.time - startTime) / transitionDuration);

            // Use the animation curve to control the interpolation
            float curveValue = transitionCurve.Evaluate(progress);

            // Smoothly interpolate the camera position and FOV
            mainCamera.transform.position = Vector3.Lerp(initialCameraPosition, new Vector3(playerTransform.position.x, playerTransform.position.y, -20), curveValue);
            mainCamera.orthographicSize = Mathf.Lerp(newFOVSize, targetCameraFov, curveValue);

            Color fogColor = fogOfWar.color;
            fogColor.a = Mathf.Lerp(initialFogAlpha, fogOfWarAlpha, curveValue);
            fogOfWar.color = fogColor;

            yield return null;
        }

        canMove = true;
        player.canMove = true;
    }
}
