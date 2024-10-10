using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float dashSpeed = 5f;         // Dash speed multiplier
    public float dashDuration = 0.2f;    // Duration of the dash
    public float dashCooldown = 1f;      // Cooldown time between dashes
    private bool isDashing = false;      // Check if player is currently dashing
    private bool canDash = true;         // Check if player can dash

    public Rigidbody2D rb;
    public Animator animator;

    public AudioSource moveSound;
    private Coroutine stepSoundCoroutine;

    public Vector2 movement;
    public Vector2 lastDirection = Vector2.up;
    private bool isMoving = false;
    private int stepsTaken = 0;

    private InputManager playerInput;
    private bool _canMove = true;

    public float maxPitch = 0.85f;
    public float minPitch = 0.65f;
    public float stepTimingAdjustment = 0.95f;
    public float stepInterval = 0.435f; // Interval between steps, decrease to make loop faster
    public bool isInteracting = false;
    public bool isAttacking = false;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();

        playerInput.OnDash += HandleDash;

        if (moveSound != null)
        {
            moveSound.loop = true;
        }
        else
        {
            Debug.LogWarning("Move sound AudioSource is not assigned!");
        }
    }

    private void OnDestroy()
    {
        playerInput.OnDash -= HandleDash;
    }

    void Update()
    {
        if (isAttacking)
        {
            movement = Vector2.zero;
            StopMovementSound();
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Speed", 0);
            return;
        }

        if (!_canMove)
        {
            StopMovementSound();
            return;
        }

        if (!isDashing)
        {
            movement = playerInput.inputVector;

            if (movement != Vector2.zero)
            {
                lastDirection = movement;

                if (!isMoving)
                {
                    isMoving = true;
                    StartMovementSound(); // Start the sound coroutine when movement starts
                }

                AdjustSoundProperties();
            }
            else if (isMoving)
            {
                isMoving = false;
                StopMovementSound(); // Stop the sound coroutine when movement stops
            }

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        if (!isInteracting && !isDashing)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        // Stop the movement sound coroutine if it's running
        StopMovementSound();

        // Set the dash direction based on the current input direction
        Vector2 dashDirection = movement.normalized;

        // Set the appropriate animation trigger based on dash direction
        if (dashDirection.x > 0)
        {
            animator.SetTrigger("DashRight");
        }
        else if (dashDirection.x < 0)
        {
            animator.SetTrigger("DashLeft");
        }

        float startTime = Time.time;

        // Dash for a limited time
        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();  // Ensure the loop yields each frame
        }

        isDashing = false;

        // Reset the triggers after the dash is complete
        animator.ResetTrigger("DashRight");
        animator.ResetTrigger("DashLeft");


        //restart movement sound
        if (movement != Vector2.zero)
        {
            StartMovementSound();
        }

        // Cooldown before the next dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;  // Reset the ability to dash
        
    }




    private void HandleDash()
    {
        if (canDash && !isDashing)
        {
            // Ensure dash always uses the latest input direction
            if (movement != Vector2.zero)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void StartMovementSound()
    {
        // Only start the coroutine if it's not already running
        if (moveSound != null && stepSoundCoroutine == null)
        {
            stepsTaken = 0;
            stepSoundCoroutine = StartCoroutine(PlayStepSound());
        }
    }

    private void StopMovementSound()
    {
        if (moveSound != null && stepSoundCoroutine != null)
        {
            StopCoroutine(stepSoundCoroutine);
            moveSound.Stop();
            stepSoundCoroutine = null;
        }
    }

    private IEnumerator PlayStepSound()
    {
        while (isMoving)
        {
            stepsTaken++;
            if (stepsTaken % 2 == 0)
            {
                moveSound.pitch *= stepTimingAdjustment;
            }
            else
            {
                moveSound.pitch = Mathf.Lerp(minPitch, maxPitch, movement.magnitude / moveSpeed);
            }

            moveSound.Play();

            yield return new WaitForSeconds(stepInterval);
        }

        stepSoundCoroutine = null; // Reset the coroutine reference when finished
    }

    private void AdjustSoundProperties()
    {
        if (moveSound != null && stepsTaken % 2 != 0)
        {
            moveSound.pitch = Mathf.Lerp(minPitch, maxPitch, movement.magnitude / moveSpeed);
        }
    }

    public void SetMovement(bool canMove)
    {
        _canMove = canMove;
        if (!canMove)
        {
            movement = Vector2.zero;
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }
}
