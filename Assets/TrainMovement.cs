using System.Collections;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public float initialSpeed = -30f; // Initial speed of the train
    public float arrivalX = -11.7f; // X position where the train should stop
    public float leaveX = -94f;
    public float stopOvershoot = 2f; // Distance the train overshoots before coming to a complete stop
    private Rigidbody2D rb; // Rigidbody component
    private float currentVelocity; // Current velocity of the train
    public static bool hasArrived = false; // To check if the train has arrived
    private bool leaveStation = false; // To check if the train should start leaving
    public CameraShake cameraShake; // Reference to the CameraShake script

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        currentVelocity = initialSpeed; // Initialize current velocity
        cameraShake = Camera.main.GetComponent<CameraShake>(); // Get the CameraShake component from the main camera
        SoundManager.PlaySound2D(SoundManager.Sound.TrainComing);
    }

    void FixedUpdate()
    {
        // Move the train towards the station
        if (!hasArrived && transform.position.x - stopOvershoot > arrivalX + stopOvershoot)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, Time.fixedDeltaTime * 10f); // Gradually decrease speed
            rb.velocity = new Vector2(currentVelocity, 0);
            if (Mathf.Abs(currentVelocity) > 5f) // Check if the train is moving fast enough to justify shaking
            {
                StartCoroutine(cameraShake.Shake(0.5f, 0.01f)); // Trigger camera shake
            }
        }
        else if (!hasArrived && transform.position.x <= arrivalX - stopOvershoot)
        {
            hasArrived = true; // Train has arrived
            StartCoroutine(DriftToStop());
        }

        // Move the train away from the station
        if (hasArrived && leaveStation)
        {
            if (transform.position.x > leaveX)
            {
                currentVelocity = Mathf.MoveTowards(currentVelocity, initialSpeed, Time.fixedDeltaTime * 2.5f); // Gradually increase speed
                rb.velocity = new Vector2(currentVelocity, 0);
                if (Mathf.Abs(currentVelocity) > 5f) // Trigger camera shake if the train is moving fast enough while leaving
                {
                    StartCoroutine(cameraShake.Shake(0.2f, 0.01f)); // Trigger camera shake
                }
            }
            else
            {
                rb.velocity = Vector2.zero; // Stop moving when past leaveX
            }
        }

    }

    IEnumerator DriftToStop()
    {
        // Allow a slight drift beyond the target stopping point
        while (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, Time.fixedDeltaTime * 5f);
            rb.velocity = new Vector2(currentVelocity, 0);
            // Trigger a gentle camera shake
            StartCoroutine(cameraShake.Shake(0.3f, 0.007f)); // Shorter duration and lower magnitude for gentle shake
            yield return null;
        }
        rb.velocity = Vector2.zero; // Finally stop the train
        Invoke("LeavingSound", 3);
        Invoke("PrepareToLeave", 5f); // Wait for 5 seconds before leaving
    }

    void LeavingSound()
    {
        SoundManager.PlaySound2D(SoundManager.Sound.TrainLeaving);
    }

    void PrepareToLeave()
    {
        leaveStation = true;
        currentVelocity = initialSpeed / 2; // Start leaving at half the initial speed
        rb.velocity = new Vector2(currentVelocity, 0);
    }
}
