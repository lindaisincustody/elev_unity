using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component

    // Flag to track whether the player is currently blocking
    private bool isBlocking = false;
    public static bool isPlaying;
    // The current direction the player is facing

    public PlayerMovement playerMovement;
    private string facingDirection = ""; // Default direction, adjust as needed

    private CloneManager cloneManager;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        isPlaying = false;
        cloneManager = FindObjectOfType<CloneManager>();

    }

    void Update()
    {
        // Check if the Left Control key is being held down
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isBlocking)
            {
                playerMovement.SetMovement(false);
                BlockInFacingDirection();
            }
        }
        else if (isBlocking)
        {
            playerMovement.SetMovement(true);
            // Stop blocking when the key is released
            ExitBlockingState();
        }

        
    }

    void FixedUpdate()
    {
        // Existing FixedUpdate code...

        // Update clones' position
        if (cloneManager != null)
        {
            cloneManager.UpdateClones();
        }
    }

    private void BlockInFacingDirection()
    {
        // Set the blocking flag to true
        isBlocking = true;

        // Activate the blocking animation based on the facing direction
        animator.SetBool($"Defend{facingDirection}", true);

        // Set general blocking flag
        animator.SetBool("IsBlocking", true);

        // Update clone blocking animations
        if (cloneManager != null)
        {
            foreach (GameObject clone in cloneManager.clones)
            {
                Animator cloneAnimator = clone.GetComponent<Animator>();
                if (cloneAnimator != null)
                {
                    // Set the same blocking animation as the player
                    cloneAnimator.SetBool($"Defend{facingDirection}", true);
                    cloneAnimator.SetBool("IsBlocking", true);
                }
            }
        }
    }

    public void ExitBlockingState()
    {
        // Reset the blocking flag
        isBlocking = false;

        // Deactivate all blocking animations
        animator.SetBool("DefendUp", false);
        animator.SetBool("DefendDown", false);
        animator.SetBool("DefendLeft", false);
        animator.SetBool("DefendRight", false);

        // Reset the general blocking flag
        animator.SetBool("IsBlocking", false);

        // Update clone blocking animations
        if (cloneManager != null)
        {
            foreach (GameObject clone in cloneManager.clones)
            {
                Animator cloneAnimator = clone.GetComponent<Animator>();
                if (cloneAnimator != null)
                {
                    // Reset all blocking animations for the clone
                    cloneAnimator.SetBool("DefendUp", false);
                    cloneAnimator.SetBool("DefendDown", false);
                    cloneAnimator.SetBool("DefendLeft", false);
                    cloneAnimator.SetBool("DefendRight", false);
                    cloneAnimator.SetBool("IsBlocking", false);
                }
            }
        }
    }

    public void SetFacingDirection(string direction)
    {
        // Method to set the facing direction externally
        facingDirection = direction;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }


}