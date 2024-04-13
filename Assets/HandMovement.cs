using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    public GameObject[] buttons; // Assign in the inspector
    public float speed = 2.0f;
    private Vector3 targetPosition;
    private float timeToNextChange = 0f;
    private float changeInterval = 2f; // Change target every 2 seconds initially
    public Animator animator1; // Assign your first object's animator in the inspector
    public Animator animator2; // Assign your second object's animator in the inspector

    public GameObject shadowHand; // Reference to the shadow hand object
    public Vector2 shadowOffset = new Vector2(0.1f, -0.1f); // Offset for the shadow hand

    private bool isMoving = true; // Control flag for movement

    void Start()
    {
        MoveToNewButton();
    }

    void Update()
    {
        if (isMoving)
        {
            // Countdown to next possible change
            timeToNextChange -= Time.deltaTime;

            // Move towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Update the position of the shadow hand with the offset
            if (shadowHand != null)
            {
                shadowHand.transform.position = new Vector2(transform.position.x + shadowOffset.x, transform.position.y + shadowOffset.y);
            }

            // If time to change or close enough to the target, switch targets
            if (timeToNextChange <= 0 || Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                MoveToNewButton();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Set isMoving to false to stop movement
            isMoving = false;

            // Set isClicked to true for both animators
            animator1.SetBool("isclicked", true);
            animator2.SetBool("isclicked", true);

            Invoke(nameof(ResetIsClicked), 0.8f);
        }
    }

    void MoveToNewButton()
    {
        if (!isMoving) return; // Do not change target if not moving

        changeInterval = Random.Range(1f, 3f);
        timeToNextChange = changeInterval;

        int randomIndex = Random.Range(0, buttons.Length);
        targetPosition = buttons[randomIndex].transform.position;

        // Debugging: Log the target position
        Debug.Log("New target position: " + targetPosition);
    }

    private void ResetIsClicked()
    {
        // Set isClicked to false for both animators
        animator1.SetBool("isclicked", false);
        animator2.SetBool("isclicked", false);

        // Resume movement
        isMoving = true;
    }
}
