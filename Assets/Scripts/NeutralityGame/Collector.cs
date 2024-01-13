using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public CollectableManager collectableManager;
    public ArrowPointer arrow;

    public Rigidbody2D playerrb;
    public GameObject playerHand;
    public Vector2 idleHandPos;
    public Vector2 rightWalkHandPos;
    public Vector2 leftWalkHandPos;

    public float throwForce = 10f;
    public float jumpForceDivider = 3f;
    public float pickUpDistance = 2f;

    private Collectable currentCollectable;

    private InputManager playerInput;
    private Vector2 movement;
    private float velocityY = 0f;

    private bool handIsOccupied = false;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
        playerInput.OnInteract += PickUp;
    }

    private void Update()
    {
        movement = new Vector2(playerInput.inputVector.x, 0f);
        changeHandPosition();
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
        velocityY = Mathf.Max(1, (playerrb.velocity.y / jumpForceDivider));
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
        print(closestCollectable);
        return closestCollectable;
    }
}
