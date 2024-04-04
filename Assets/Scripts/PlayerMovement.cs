using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;
    private Vector2 lastDirection = Vector2.up;

    private InputManager playerInput;
    private bool _canMove = true;

    private BattlePlayerController battlePlayerController;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
        battlePlayerController = GetComponent<BattlePlayerController>();
    }


    void Update()
    {
        if (!_canMove)
            return;

        // Read movement input
        movement = playerInput.inputVector;

        // Update the animator with movement data
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement != Vector2.zero)
        {
            lastDirection = movement; // Always update the last direction if there's movement

            if (BattlePlayerController.isPlaying && battlePlayerController.IsBlocking())
            {
                battlePlayerController.ExitBlockingState();
            }
        }

        // Update the facing direction in the battle controller
        UpdateFacingDirection();
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
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

    private void UpdateFacingDirection()
    {
        if (battlePlayerController != null)
        {
            if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            {
                // Horizontal movement is dominant
                battlePlayerController.SetFacingDirection(lastDirection.x > 0 ? "Right" : "Left");
            }
            else
            {
                // Vertical movement is dominant
                battlePlayerController.SetFacingDirection(lastDirection.y > 0 ? "Up" : "Down");
            }

            // Debug log to check the values
      
        }
    }
}
