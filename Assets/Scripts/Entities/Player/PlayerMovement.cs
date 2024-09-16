using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed = 1f;
    public Rigidbody2D rb;
    public Animator animator;
    protected Animator animSync;
    protected Animator doorAnimator;
    public GameObject doorObj;

    public AudioSource moveSound;

    private Vector2 movement;
    private Vector2 lastDirection = Vector2.up;
    private bool isMoving = false;
    private int stepsTaken = 0;

    private InputManager playerInput;
    private bool _canMove = true;

    public float maxPitch = 0.85f;
    public float minPitch = 0.65f;
    public float stepTimingAdjustment = 0.95f;
    public float stepInterval = 0.435f; // Interval between steps, decrease to make loop faster
    public bool isInteracting = false;



    private void Awake()
    {
        playerInput = GetComponent<InputManager>();

        if (moveSound != null)
        {
            moveSound.loop = true;
        }
        else
        {
            Debug.LogWarning("Move sound AudioSource is not assigned!");
        }
    }

    void Update()
    {
        if (!_canMove)
        {
            StopMovementSound();
            return;
        }

            movement = playerInput.inputVector;

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);


        if (movement != Vector2.zero)
        {
            lastDirection = movement;
            if (!isMoving)
            {
                StartMovementSound();
                isMoving = true;
            }
            AdjustSoundProperties();

          
        }
        else if (isMoving)
        {
            StopMovementSound();
            isMoving = false;
        }


    }

    void FixedUpdate()
    {
        if (!isInteracting)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator PlayStepSound()
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

        // Wait for the specified interval before playing the next step sound
        yield return new WaitForSeconds(stepInterval);

        // Check if the player is still moving before playing the next sound
        if (isMoving)
        {
            StartCoroutine(PlayStepSound());
        }
    }

    private void StartMovementSound()
    {
        if (moveSound != null && !isMoving)
        {
            stepsTaken = 0;
            StartCoroutine(PlayStepSound());
        }
    }

    private void StopMovementSound()
    {
        if (moveSound != null && isMoving)
        {
            moveSound.Stop();
            StopAllCoroutines(); // Stop the coroutine when the player stops moving
        }
    }

    private void AdjustSoundProperties()
    {
        // This ensures pitch adjustments occur only when the sound is not being adjusted for step timing
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