using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutterMover : MonoBehaviour
{
    [SerializeField] InputManager playerInput;
    [SerializeField] int leftBorder = -5;
    [SerializeField] int rightBorder = 5;
    public float moveSpeed = 1f;
    public float rotationSpeed = 50f;

    bool movingRight = true;
    private float horizontalInput;
    private float verticalInput;

    void Update()
    {
        horizontalInput = playerInput.inputVector.x * moveSpeed;
        float newPositionX = transform.position.x + horizontalInput * Time.deltaTime;
        newPositionX = Mathf.Clamp(newPositionX, leftBorder, rightBorder); // Clamp the x position

        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);

        verticalInput = playerInput.inputVector.y;
        transform.Rotate(Vector3.forward, verticalInput * rotationSpeed * Time.deltaTime);


        if (newPositionX >= rightBorder)
        {
            newPositionX = rightBorder;
            movingRight = false;
        }
        else if (newPositionX <= leftBorder)
        {
            newPositionX = leftBorder;
            movingRight = true;
        }

        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);

    }
}
/*
transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

float movement = moveSpeed * Time.deltaTime * (movingRight ? 1 : -1);
float newPositionX = transform.position.x + movement;

if (newPositionX >= rightBorder)
{
    newPositionX = rightBorder;
    movingRight = false;
}
else if (newPositionX <= leftBorder)
{
    newPositionX = leftBorder;
    movingRight = true;
}

transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);*/