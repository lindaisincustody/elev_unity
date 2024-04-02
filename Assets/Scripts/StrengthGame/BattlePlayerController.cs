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
    private string facingDirection = "Up"; // Default direction, adjust as needed

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        isPlaying = false;

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

    private void BlockInFacingDirection()
    {
        // Set the blocking flag to true
        isBlocking = true;

        // Activate the blocking animation based on the facing direction
        animator.SetBool($"Defend{facingDirection}", true);

        // Set general blocking flag
        animator.SetBool("IsBlocking", true);
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