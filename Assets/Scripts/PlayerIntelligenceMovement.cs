using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovementIntelli : MonoBehaviour
{
    public float moveSpeedIntelli = 1f;
    public Rigidbody2D rbIntelli;
    public Animator animatorIntelli;

    private Vector2 movementIntelli;

    private InputManager playerInputIntelli;
    private GameController gameController;

    private void Awake()
    {
        playerInputIntelli = GetComponent<InputManager>();
        gameController = FindObjectOfType<GameController>();
    }


    void Update()
    {
        movementIntelli = playerInputIntelli.inputVector;
        animatorIntelli.SetFloat("Horizontal", movementIntelli.x);
        animatorIntelli.SetFloat("Vertical", movementIntelli.y);
        animatorIntelli.SetFloat("Speed", movementIntelli.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rbIntelli.MovePosition(rbIntelli.position + movementIntelli * moveSpeedIntelli * Time.fixedDeltaTime);
    }
}
