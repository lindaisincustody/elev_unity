using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
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
    private Rigidbody2D[] collectableRbs;

    private InputManager playerInput;
    private Vector2 movement;
    private float velocityY = 0f;

    private bool handIsOccupied = false;

    [SerializeField] LayerMask consumableLayer;

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
        velocityY = Mathf.Max(1, (playerrb.velocity.y / jumpForceDivider));
        //velocityY = Mathf.Clamp(rb.velocity.y / jumpForceDivider, -1f, 1f);
        colRb.AddForce(arrow.throwDirection * throwForce * velocityY, ForceMode2D.Impulse);
        //colRb.AddForce(rb.velocity * throwForce, ForceMode2D.Impulse);


        handIsOccupied = false;
    }

    private void PickUpConsumable()
    {
        if (!getClosestCollectable())
            return;

        handIsOccupied = true;
        foreach (var item in collectableRbs)
        {
            item.simulated = false;
        }
    }

    private bool getClosestCollectable()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 1f, consumableLayer);
        if (collider == null)
            return false;

        GameObject collectable = collider.transform.parent.gameObject;
        collectableRbs = collectable.GetComponentsInChildren<Rigidbody2D>();
        return true;
    }
}
