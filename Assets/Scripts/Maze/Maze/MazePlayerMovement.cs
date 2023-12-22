using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazePlayerMovement : MonoBehaviour
{
    public bool canMove = false;
    public float moveSpeed = 1f;
    public float boostForce = 2f; // Adjust this value to control the initial boost force
    public float boostDecay = 2f;  // Adjust this value to control how quickly the boost decays
    public float boostDuration = 2f;
    public Rigidbody2D rb;

    public InputManager playerInput;

    public MazeGenerator mazeGenerator;
    private Vector2 movement;

    private LineRenderer leashRenderer;
    private DistanceJoint2D joint;

    private bool isLeashed = false;
    private bool isBoosting = false;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
        playerInput.OnBoost += Boost;
        playerInput.OnInteract += ActivateShortestPath;
        playerInput.OnCancel += DeactivateShortestPath;

        joint = GetComponent<DistanceJoint2D>();
        leashRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!canMove)
            return;
        movement = playerInput.inputVector;

        if (isLeashed)
            leashRenderer.SetPosition(0, transform.position);
    }

    void FixedUpdate()
    {
        if (isBoosting)
        {
            // Apply boosted movement
            rb.velocity = movement.normalized * boostForce;
        }
        else
        {
            // Apply normal movement
            rb.velocity = movement.normalized * moveSpeed;
        }
    }

    private void Boost()
    {
        if (!isBoosting)
        {
            // Apply the boost
            isBoosting = true;
            rb.velocity = movement.normalized * boostForce;

            // Start a coroutine to end the boost after a specified boostDuration
            StartCoroutine(EndBoost());
        }
    }

    private IEnumerator EndBoost()
    {
        yield return new WaitForSeconds(boostDuration);

        // Reset to normal movement speed
        isBoosting = false;
        rb.velocity = movement.normalized * moveSpeed;
    }

    private void ActivateShortestPath()
    {
        mazeGenerator.Activate(10);
    }

    private void DeactivateShortestPath()
    {
        mazeGenerator.DeactivateShortestPath();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MazeEnd"))
            SceneManager.LoadScene("SampleScene");
    }

    public void LeashToObject(Rigidbody2D rbToLeash, Transform enemyPos)
    {
        if (isLeashed)
            return;
        isLeashed = true;
        joint.enabled = true;
        joint.connectedBody = rbToLeash;
        leashRenderer.enabled = true;
        leashRenderer.SetPosition(1, enemyPos.position);

        StartCoroutine(Unleash());
    }

    private IEnumerator Unleash()
    {
        yield return new WaitForSeconds(7f);
        joint.enabled = false;
        joint.connectedBody = null;
        leashRenderer.enabled = false;
        // Time to Escape
        yield return new WaitForSeconds(2f);
        isLeashed = false;
    }
}
