using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNeutrality : MonoBehaviour
{
    public CollectableManager collectableManager;
    public ArrowPointer arrow;
    public GameObject playerHand;
    public float moveSpeed = 1f;
    public float pickUpDistance = 2f;
    public float jumpForce = 5f;
    public float jumpCd = 2f;
    public float throwForce = 10f;
    public float jumpForceDivider = 3f;

    public Vector2 idleHandPos;
    public Vector2 rightWalkHandPos;
    public Vector2 leftWalkHandPos;

    private Vector2 movement;
    private float velocityY = 0f;
    private bool handIsOccupied = false;
    private Collectable currentCollectable;
    private Rigidbody2D rb;
    private Animator animator;

    private InputManager playerInput;
    private bool canJump = true;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
        playerInput.OnInteract += PickUp;
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
        changeHandPosition();
    }

    void FixedUpdate()
    {
        if (rb.velocity.y == 0)
        {
            rb.AddForce(new Vector2(movement.x * moveSpeed, 0f), ForceMode2D.Force);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

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
        Rigidbody2D colRb = currentCollectable.GetComponent<Rigidbody2D>();
        colRb.bodyType = RigidbodyType2D.Dynamic;
        currentCollectable.transform.parent = collectableManager.collectablesHolder;
        velocityY = Mathf.Max(1, (rb.velocity.y / jumpForceDivider));
        //velocityY = Mathf.Clamp(rb.velocity.y / jumpForceDivider, -1f, 1f);
        colRb.AddForce(arrow.throwDirection * throwForce * velocityY, ForceMode2D.Impulse);
        //colRb.AddForce(rb.velocity * throwForce, ForceMode2D.Impulse);


        handIsOccupied = false;
    }

    private void PickUpConsumable()
    {
        Collectable collectable = getClosestCollectable();
        if (collectable == null)
            return;

        collectable.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
