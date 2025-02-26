using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private int dashCount = 0;
    public int maxDashCount = 1;
    public float baseMoveSpeed = 5.5f;
    public float dashSpeed = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;
    private bool isDashing = false;
    public bool canDash = true;

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
    public float stepInterval = 0.435f;
    public bool isInteracting = false;
    public bool isAttacking = false;

    public Action OnDash;

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
            // movement = Vector2.zero;
            baseMoveSpeed = 2.75f;
            StopMovementSound();
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Speed", 0);
            return;
        }
        else
        {
            baseMoveSpeed = 5.5f;
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
                    StartMovementSound();
                }

                AdjustSoundProperties();
            }
            else if (isMoving)
            {
                isMoving = false;
                StopMovementSound();
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
            // Use adjustedMoveSpeed to move the player
            rb.MovePosition(rb.position + movement * baseMoveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash()
    {
        OnDash?.Invoke();

        isDashing = true;
        canDash = false;
        dashCount++;

        StopMovementSound();

        Vector2 dashDirection = movement.normalized;

        if (dashDirection.x > 0)
        {
            animator.SetTrigger("DashRight");
        }
        else if (dashDirection.x < 0)
        {
            animator.SetTrigger("DashLeft");
        }
        else if (dashDirection.y > 0)
        {
            animator.SetTrigger("DashBack");
        }
        else if (dashDirection.y < 0)
        {
            animator.SetTrigger("DashFront");
        }

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        animator.ResetTrigger("DashRight");
        animator.ResetTrigger("DashLeft");
        animator.ResetTrigger("DashBack");
        animator.ResetTrigger("DashFront");

        if (movement != Vector2.zero)
        {
            StartMovementSound();
        }

        if (dashCount < maxDashCount)
        {
            canDash = true;
        }
        else
        {
            yield return new WaitForSeconds(dashCooldown);
            dashCount = 0; // Reset for next dashes
            canDash = true;
        }
    }

    private void HandleDash()
    {
        if (SanityBar.instance == null || !SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
        {
            return;
        }

        if (canDash && !isDashing && !isAttacking)
        {
            if (movement != Vector2.zero)
            {
                StartCoroutine(Dash());
            }
        }
    }


    private void StartMovementSound()
    {
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
                moveSound.pitch = Mathf.Lerp(minPitch, maxPitch, movement.magnitude / baseMoveSpeed);
            }

            moveSound.Play();

            yield return new WaitForSeconds(stepInterval);
        }

        stepSoundCoroutine = null;
    }

    private void AdjustSoundProperties()
    {
        if (moveSound != null && stepsTaken % 2 != 0)
        {
            moveSound.pitch = Mathf.Lerp(minPitch, maxPitch, movement.magnitude / baseMoveSpeed);
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