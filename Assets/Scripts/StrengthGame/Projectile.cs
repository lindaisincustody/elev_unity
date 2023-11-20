using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public Transform target;

    private Collider2D playerCollider; // Store the reference to the player collider

    void Start()
    {
        // Set the initial velocity towards the target
        Vector2 direction = (target.position - transform.position).normalized;
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    void Update()
    {
        // You can add additional logic here if needed
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the projectile collided with the heart
        if (other.CompareTag("Heart"))
        {
            // Implement any logic you need when the projectile hits the heart
            Debug.Log("Projectile hit the heart!");

            // Destroy the projectile
            Destroy(gameObject);
        }
        // Blocking
        else if (other.CompareTag("Player"))
        {
            // Check if the Shift key is being held down
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Check if the player is blocking
                if (other.GetComponent<BattlePlayerController>() != null && other.GetComponent<BattlePlayerController>().IsBlocking())
                {
                    // Store the reference to the player collider
                    playerCollider = other;

                    // Implement any logic you need when the projectile is blocked
                    Debug.Log("Projectile blocked by player!");

                    // Play the blocking animation on the player
                    // You may need to set up a parameter in the player animator for blocking
                    other.GetComponent<Animator>().SetBool("IsBlocking", true);

                    // Destroy the projectile
                    Destroy(gameObject);

                    // Delay the execution of SetBool("IsBlocking", false) by 1 second
                    Invoke("ResetBlocking", 1f);
                }
            }
        }
    }

    // Function to reset the blocking state after 1 second
    void ResetBlocking()
    {
        // Use the stored reference to reset the blocking state
        playerCollider.GetComponent<Animator>().SetBool("IsBlocking", false);
    }
}
