using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component

    // Flag to track whether the player is currently blocking
    private bool isBlocking = false;
    
    void OnTriggerStay2D(Collider2D other)
    {
        // Check if the player is in the left zone
        if (other.CompareTag("LeftZone"))
        {
            // Check if the Shift key is being held down
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Play the left zone animation
                animator.SetBool("LeftZone", true);
                animator.SetBool("IsHoldingShift", true);
                animator.SetBool("IsBlocking", false);
                // Set the blocking flag to true
                isBlocking = true;
            }
            else
            {
                // Stop the left zone animation
                animator.SetBool("LeftZone", false);

                // Set the blocking flag to false
                isBlocking = false;
            }
        }
        // Check if the player is in the right zone
        else if (other.CompareTag("RightZone"))
        {
            // Check if the Shift key is being held down
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Play the left zone animation
                animator.SetBool("RightZone", true);
                animator.SetBool("IsHoldingShift", true);
                animator.SetBool("IsBlocking", false);

                // Set the blocking flag to true
                isBlocking = true;
            }
            else
            {
                // Stop the left zone animation
                animator.SetBool("RightZone", false);

                // Set the blocking flag to false
                isBlocking = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset animation and blocking flag when leaving the trigger zone
        animator.SetBool("LeftZone", false);
        animator.SetBool("RightZone", false);
        
        isBlocking = false;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }
}
