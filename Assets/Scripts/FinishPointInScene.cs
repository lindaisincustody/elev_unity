using UnityEngine;

public class FinishPointInScene : MonoBehaviour
{
    public float scene_X;
    public float scene_Y;
    public float iconY;
    public GameObject iconPrefab;
    public Vector2 cameraMinBounds;
    public Vector2 cameraMaxBounds;
    public SmoothCameraFollow cameraFollowScript;

    private GameObject instantiatedIcon = null;
    private Transform playerTransform = null;
    private bool playerIsInTrigger = false;

    private void Awake()
    {
        var inputManager = FindObjectOfType<InputManager>();
        if (inputManager != null)
        {
            inputManager.OnInteract += HandleInteract;
        }
    }

    private void Start()
    {
        if (cameraFollowScript != null)
        {
            // Set the camera bounds from the manually specified values
            cameraFollowScript.SetMinBounds(cameraMinBounds);
            cameraFollowScript.SetMaxBounds(cameraMaxBounds);
        }
    }

    private void OnDestroy()
    {
        var inputManager = FindObjectOfType<InputManager>();
        if (inputManager != null)
        {
            inputManager.OnInteract -= HandleInteract;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            playerTransform = collision.transform; // Store the player's transform
            if (iconPrefab != null && instantiatedIcon == null)
            {
                Vector3 iconPosition = playerTransform.position + new Vector3(0, 1.0f, 0); // Adjust this offset as needed
                instantiatedIcon = Instantiate(iconPrefab, iconPosition, Quaternion.identity);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            playerTransform = null; // Clear the player's transform
            if (instantiatedIcon != null)
            {
                Destroy(instantiatedIcon);
                instantiatedIcon = null; // Reset the reference
            }
        }
    }

    private void Update()
    {
        if (playerIsInTrigger && instantiatedIcon != null && playerTransform != null)
        {
            // Update the icon's position to be above the player
            instantiatedIcon.transform.position = playerTransform.position + new Vector3(0, iconY, 0); // Adjust Y offset as needed
        }
    }

    private void HandleInteract()
    {
        if (playerIsInTrigger && SceneController.instance != null)
        {
            float clampedX = Mathf.Clamp(scene_X, cameraMinBounds.x, cameraMaxBounds.x);
            float clampedY = Mathf.Clamp(scene_Y, cameraMinBounds.y, cameraMaxBounds.y);

            StartCoroutine(SceneController.instance.LoadInScene(clampedX, clampedY));
        }
    }
}
