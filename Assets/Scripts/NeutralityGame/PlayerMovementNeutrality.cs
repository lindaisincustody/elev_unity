using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNeutrality : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 5f;
    public float jumpCd = 2f;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    private InputManager playerInput;
    private bool canJump = true;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
        playerInput.OnJump += Jump;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        movement = new Vector2(playerInput.inputVector.x, 0f);
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        if (rb.velocity.y == 0)
        {
            rb.AddForce(new Vector2(movement.x * moveSpeed, 0f), ForceMode2D.Force);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

    }

    private void Jump()
    {
        if (!canJump)
            return;
        canJump = false;
        Invoke("ResetJump", jumpCd);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }
}
