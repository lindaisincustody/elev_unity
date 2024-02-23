using UnityEngine;

public class FinishPointInScene : MonoBehaviour
{
    public float scene_X;
    public float scene_Y;
    public GameObject iconPrefab; // Assign your icon prefab here in the inspector
    private GameObject instantiatedIcon = null; // To hold the instantiated icon
    private Transform playerTransform = null; // To track the player's position

    private bool playerIsInTrigger = false;

    private void Awake()
    {
        var inputManager = FindObjectOfType<InputManager>();
        if (inputManager != null)
        {
            inputManager.OnInteract += HandleInteract;
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
            instantiatedIcon.transform.position = playerTransform.position + new Vector3(0, 1.0f, 0); // Adjust Y offset as needed
        }
    }

    private void HandleInteract()
    {
        if (playerIsInTrigger && SceneController.instance != null)
        {
            // Start the coroutine properly
            StartCoroutine(SceneController.instance.LoadInScene(scene_X, scene_Y));
        }
    }
}
