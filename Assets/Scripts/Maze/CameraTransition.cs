using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public Volume volume;
    private Vignette vignette;

    public Color initialBackgroundColor = Color.black;
    public Color targetBackgroundColor = Color.blue;

    public bool canMove = false;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        volume.profile.TryGet(out vignette);
    }

    private void Update()
    {
        if (canMove)
            mainCamera.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -20);
    }

    public void SetCamera(int mazeWidth, int mazeHeight, int cellSize)
    {
        initialCameraPosition = new Vector3(mazeWidth * cellSize / 2 - 5, mazeHeight * cellSize / 2 - 2, -20);
        newFOVSize = (Mathf.Max(mazeHeight, mazeWidth) / 30.0f) * maxCameraFOV;
        mainCamera.transform.position = initialCameraPosition;
        mainCamera.orthographicSize = newFOVSize;

        // Set the initial background color
        mainCamera.backgroundColor = initialBackgroundColor;
    }

    public void StartZoomIn()
    {
        StartCoroutine(ZoomIn());
    }

    private IEnumerator ZoomIn()
    {
        yield return new WaitForSeconds(1f);
        float startTime = Time.time;
        float progress = 0;

        while (progress < 1)
        {
            progress = Mathf.Clamp01((Time.time - startTime) / transitionDuration);

            float curveValue = transitionCurve.Evaluate(progress);

            mainCamera.transform.position = Vector3.Lerp(initialCameraPosition, new Vector3(playerTransform.position.x, playerTransform.position.y, -20), curveValue);
            mainCamera.orthographicSize = Mathf.Lerp(newFOVSize, targetCameraFov, curveValue);

            mainCamera.backgroundColor = Color.Lerp(initialBackgroundColor, targetBackgroundColor, curveValue);

            vignette.intensity.value = Mathf.Lerp(0, 0.5f, curveValue);

            yield return null;
        }

        canMove = true;
        player.canMove = true;
    }
}
