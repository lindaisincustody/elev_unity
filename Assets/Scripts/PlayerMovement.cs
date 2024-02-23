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

    private InputManager playerInput;
    private bool _canMove = true;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
    }


    void Update()
    {
        if (!_canMove)
            return;

        movement = playerInput.inputVector;
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
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
}
