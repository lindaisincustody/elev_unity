using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Rigidbody2D rb;

    public InputManager playerInput;

    public MazeGenerator mazeGenerator;
    private Vector2 movement;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
    }


    void Update()
    {
        movement = playerInput.inputVector;

        if (Input.GetKeyDown(KeyCode.E))
            ActivateShortestPath();
        if (Input.GetKeyUp(KeyCode.E))
            DeactivateShortestPath();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void ActivateShortestPath()
    {
        mazeGenerator.ActivateShortestPath();
    }

    private void DeactivateShortestPath()
    {
        mazeGenerator.DeactivateShortestPath();
    }
}