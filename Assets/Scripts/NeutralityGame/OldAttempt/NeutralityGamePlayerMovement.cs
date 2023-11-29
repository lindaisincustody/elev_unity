using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralityGamePlayerMovement : MonoBehaviour
{
    public CollectableManager collectableManager;
    public GameObject playerHand;
    public float moveSpeed = 1f;
    public float pickUpDistance = 2f;
    public float jumpForce = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    public Vector2 idleHandPos;
    public Vector2 rightWalkHandPos;
    public Vector2 leftWalkHandPos;

    public float gravity = 0.2f;
    public float maxGravity = 2f;
    private float currentGravity = 0f;

    public int groundHitCount = 0;
    
    private Vector2 movement;
    private bool allowYAxisMovement = true;
    private bool handIsOccupied = false;
    private Collectable currentCollectable;

    private InputManager playerInput;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
        playerInput.OnInteract += PickUp;
        playerInput.OnJump += Jump;
    }

    private void Start()
    {
        SetYAxisMovement(false);
    }

    void Update()
    {
        if (allowYAxisMovement)
        {
            movement = playerInput.inputVector;
        }
        else
        {
            movement = new Vector2(playerInput.inputVector.x, 0f);
            rb.gravityScale = currentGravity;
        }

        if (allowYAxisMovement || groundHitCount > 0)
        {
            currentGravity = 0f;
        }
        else
        {
            if (currentGravity > maxGravity)
                currentGravity = maxGravity;
            currentGravity += Time.deltaTime * gravity;
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        changeHandPosition();
    }

    void FixedUpdate()
    {
        Vector2 normalizedMovement = movement.normalized;
        rb.MovePosition(rb.position + normalizedMovement * moveSpeed * Time.fixedDeltaTime);
    }

    public void GetOnALadder()
    {
        SetYAxisMovement(true);
        currentGravity = 0f;
        rb.gravityScale = 0f;
    }

    public void GetOffALadder()
    {
        SetYAxisMovement(false);
    }

    private void SetYAxisMovement(bool isEnabled)
    {
        allowYAxisMovement = isEnabled;
    }

    private void changeHandPosition()
    {
        Vector2 newHandPos = Vector2.zero;

        // Check the horizontal movement to determine the animation state
        if (movement.x > 0.1f)
        {
            // Moving right
            newHandPos = rightWalkHandPos;
        }
        else if (movement.x < -0.1f)
        {
            // Moving left
            newHandPos = leftWalkHandPos;
        }
        else
        {
            // Idle (no horizontal movement)
            newHandPos = idleHandPos;
        }

        // Apply the new hand position
        playerHand.transform.localPosition = newHandPos;
    }


    private void PickUp()
    {
        if (handIsOccupied)
            DropConsumable();
        else
            PickUpConsumable();
    }

    private void DropConsumable()
    {
        if (currentCollectable == null)
            return;
        currentCollectable.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        currentCollectable.transform.parent = collectableManager.collectablesHolder;
        handIsOccupied = false;
    }

    private void PickUpConsumable()
    {
        Collectable collectable = getClosestCollectable();
        if (collectable == null)
            return;

        handIsOccupied = true;
        currentCollectable = collectable;
        collectable.transform.parent = playerHand.transform;
        collectable.transform.position = playerHand.transform.position;
        collectable.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    private Collectable getClosestCollectable()
    {
        Collectable closestCollectable = null;
        float closestDistance = int.MaxValue;
        foreach (var collectable in collectableManager.collectables)
        {
            if (Vector2.Distance(collectable.transform.position, gameObject.transform.position) < closestDistance)
            {
                closestDistance = Vector2.Distance(collectable.transform.position, gameObject.transform.position);
                if (closestDistance < pickUpDistance)
                    closestCollectable = collectable;
            }
        }
        return closestCollectable;
    }

    private void Jump()
    {
        Debug.Log("Jump");

        Debug.Log("Before Jump - Velocity: " + rb.velocity);

        // Apply a vertical force for jumping
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

        Debug.Log("After Jump - Velocity: " + rb.velocity);

    }
}
