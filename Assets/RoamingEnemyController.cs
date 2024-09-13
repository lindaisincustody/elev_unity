using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal; // Needed for Light2D
public class EnemyController : MonoBehaviour
{
    public GameObject sparkleEffect;  // Reference to the sparkle effect object
    public float moveSpeed = 2f;      // Movement speed of the enemy
    public float changeTargetTime = 2f; // Time after which the enemy changes its target point
    public Light2D enemyLight;
    public Vector2 roamingAreaCenter = new Vector2(25f, 1f); // Center of the roaming area
    public Vector2 roamingAreaSize = new Vector2(50f, 5f); // Size of the roaming area (width, height)

    private bool isInUnderworld = false; // Track if the enemy is in the underworld state
    private SpriteRenderer spriteRenderer;  // Reference to enemy's sprite renderer
    private Collider2D enemyCollider;  // Reference to enemy's collider
    private Vector2 targetPosition;  // Target position for the enemy to move towards
    private float targetChangeTimer = 0f; // Timer to change the target position
    public float underworldLightIntensity = 5f;

    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        // Initially, make sure sparkle effect is off and enemy is hidden
        if (SanityBar.instance.currentSanity > 50)
        {
            ShowSparkle();
        }
        else
        {
            ShowEnemy();
        }

        // Set an initial random target position within the roaming area
        SetRandomTargetPosition();
    }

    void Update()
    {
        // Update movement and sanity-based behavior
        UpdateSanityState();
        SmoothMovement();
    }

    private void UpdateSanityState()
    {
        // Update the enemy state based on the sanity level
        if (SanityEffectHandler.IsPlayerInUnderworld && !isInUnderworld)
        {
            ShowEnemy();  // Switch to enemy mode
        }
        else if (!SanityEffectHandler.IsPlayerInUnderworld && isInUnderworld)
        {
            ShowSparkle();  // Switch to sparkle mode
        }
    }

    private void SmoothMovement()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if it's time to change the target position
        targetChangeTimer += Time.deltaTime;
        if (targetChangeTimer >= changeTargetTime || Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
            targetChangeTimer = 0f;
        }
    }

    private void SetRandomTargetPosition()
    {
        // Generate a random point within the defined roaming area size
        float randomX = Random.Range(roamingAreaCenter.x - roamingAreaSize.x / 2, roamingAreaCenter.x + roamingAreaSize.x / 2);
        float randomY = Random.Range(roamingAreaCenter.y - roamingAreaSize.y / 2, roamingAreaCenter.y + roamingAreaSize.y / 2);

        targetPosition = new Vector2(randomX, randomY);
    }

    private void ShowEnemy()
    {
        // Make enemy visible and active
        isInUnderworld = true;
        spriteRenderer.enabled = true;
        enemyCollider.enabled = true;

        if (sparkleEffect != null)
        {
            sparkleEffect.SetActive(false);  // Hide the sparkle effect
        }
        // Decrease the light intensity when in the underworld
        if (enemyLight != null)
        {
            enemyLight.intensity = underworldLightIntensity;
        }
    }

    private void ShowSparkle()
    {
        if (enemyLight != null)
        {
            enemyLight.intensity = 1;
        }
        // Switch to sparkle mode, hide enemy
        isInUnderworld = false;
        spriteRenderer.enabled = false;  // Hide the enemy sprite
        enemyCollider.enabled = false;   // Disable enemy's interaction

        if (sparkleEffect != null)
        {
            sparkleEffect.SetActive(true);  // Show the harmless sparkle effect
        }
    }
}
