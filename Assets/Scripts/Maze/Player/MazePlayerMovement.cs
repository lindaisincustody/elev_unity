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
    public Animator animator;

    public InputManager playerInput;
    public MazeManager mazeManager;
    public MazeGenerator mazeGenerator;
    private Vector2 movement;

    private LineRenderer leashRenderer;
    private DistanceJoint2D joint;

    private bool isLeashed = false;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
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

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (isLeashed)
            leashRenderer.SetPosition(0, transform.position);
    }

    void FixedUpdate()
    {
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

    public void StopPlayer()
    {
        canMove = false;
        rb.velocity = Vector3.zero;
        movement = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MazeEnd"))
        {
            mazeManager.Win();
        }
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
