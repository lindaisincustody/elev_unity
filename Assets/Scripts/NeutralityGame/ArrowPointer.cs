using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform player; // Assign the player's transform in the Unity editor
    public float rotationSpeed = 30f; // Adjust the rotation speed as needed
    public float maxAngle = 30f; // Adjust the maximum angle to switch direction as needed
    public Vector2 throwDirection;

    private float angle = 90f;
    private int rotationDirection = 1; // 1 for clockwise, -1 for counterclockwise

    void Update()
    {
        RotateObjectAroundPlayer();
    }

    void RotateObjectAroundPlayer()
    {
        // Calculate the position of the object based on the angle
        float x = player.position.x + Mathf.Cos(Mathf.Deg2Rad * angle) * 2f; // Adjust the radius as needed
        float y = player.position.y + Mathf.Sin(Mathf.Deg2Rad * angle) * 2f; // Adjust the radius as needed

        // Set the object's position
        transform.position = new Vector3(x, y, 0f);

        // Increment the angle based on the rotation speed and direction
        angle += rotationSpeed * rotationDirection * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        if (Mathf.Abs(angle - 90) > maxAngle)
        {
            rotationDirection *= -1; // Switch direction
        }

        throwDirection = (transform.position - player.position).normalized;
    }
}
