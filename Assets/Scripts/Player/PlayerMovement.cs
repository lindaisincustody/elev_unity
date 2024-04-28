using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Rigidbody2D rb;
    public Animator animator;
    protected Animator animSync;
    protected Animator doorAnimator;
    public GameObject doorObj;

    public AudioSource moveSound;

    private Vector2 movement;
    private Vector2 lastDirection = Vector2.up;
    private bool isMoving = false;
    private int stepsTaken = 0;

    public InputManager playerInput;
    public bool _canMove = true;

    public BattlePlayerController battlePlayerController;

    public float maxPitch = 0.85f;
    public float minPitch = 0.65f;
    public float stepTimingAdjustment = 0.95f;
    public float stepInterval = 0.435f; // Interval between steps, decrease to make loop faster
    public bool isInteracting = false;

    private void Awake()
    {
        playerInput = GetComponent<InputManager>();
        battlePlayerController = GetComponent<BattlePlayerController>();

        //animSync = GameObject.Find("AnimSync").GetComponent<Animator>();
        //doorAnimator = GameObject.Find("Door").GetComponent<Animator>();


        if (moveSound != null)
        {
            moveSound.loop = true;
        }
        else
        {
            Debug.LogWarning("Move sound AudioSource is not assigned!");
        }
    }

    void Update()
    {
        //if (animSync != null && isInteracting)
        //{
        //    float syncTime = animSync.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //    animator.Play(0, -1, syncTime);
        //    doorAnimator.Play(0, -1, syncTime);
        //}
        if (!_canMove)
        {
            StopMovementSound();
            return;
        }
        if (!isInteracting)
        {
           // movement = playerInput.inputVector;

            //animator.SetFloat("Horizontal", movement.x);
            //animator.SetFloat("Vertical", movement.y);
            //animator.SetFloat("Speed", movement.sqrMagnitude);
        }

        if (BattlePlayerController.isPlaying && battlePlayerController.IsBlocking())
        {
            battlePlayerController.ExitBlockingState();
        }

        if (movement != Vector2.zero)
        {
            lastDirection = movement;
            if (!isMoving)
            {
                StartMovementSound();
                isMoving = true;
            }
            AdjustSoundProperties();

          
        }
        else if (isMoving)
        {
            StopMovementSound();
            isMoving = false;
        }

        UpdateFacingDirection();
    }

    void FixedUpdate()
    {
        if (!isInteracting)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator PlayStepSound()
    {
        stepsTaken++;
        if (stepsTaken % 2 == 0)
        {
            moveSound.pitch *= stepTimingAdjustment;
        }
        else
        {
            moveSound.pitch = Mathf.Lerp(minPitch, maxPitch, movement.magnitude / moveSpeed);
        }

        moveSound.Play();

        // Wait for the specified interval before playing the next step sound
        yield return new WaitForSeconds(stepInterval);

        // Check if the player is still moving before playing the next sound
        if (isMoving)
        {
            StartCoroutine(PlayStepSound());
        }
    }

    public void StartMovementSound()
    {
        if (moveSound != null && !isMoving)
        {
            stepsTaken = 0;
            StartCoroutine(PlayStepSound());
        }
    }

    public void StopMovementSound()
    {
        if (moveSound != null && isMoving)
        {
            moveSound.Stop();
            StopAllCoroutines(); // Stop the coroutine when the player stops moving
        }
    }

    public void AdjustSoundProperties()
    {
        // This ensures pitch adjustments occur only when the sound is not being adjusted for step timing
        if (moveSound != null && stepsTaken % 2 != 0)
        {
            moveSound.pitch = Mathf.Lerp(minPitch, maxPitch, movement.magnitude / moveSpeed);
        }
    }

    public void SetMovement(bool canMove)
    {
        _canMove = canMove;
        if (!canMove)
        {
            if (!isInteracting)
            {
                movement = Vector2.zero;
                animator.SetFloat("Horizontal", movement.x);
                animator.SetFloat("Vertical", movement.y);
                animator.SetFloat("Speed", movement.sqrMagnitude);
            }
        }
    }

    private void UpdateFacingDirection()
    {
        if (battlePlayerController != null)
        {
            if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            {
                battlePlayerController.SetFacingDirection(lastDirection.x > 0 ? "Right" : "Left");
            }
            else
            {
                battlePlayerController.SetFacingDirection(lastDirection.y > 0 ? "Up" : "Down");
            }
        }
    }

    public void StartInteractionSequence()
    {
        StartCoroutine(InteractionSequence());
    }

    private IEnumerator InteractionSequence()
    {
        GetComponent<AnimatorSynchroniser>().AddAnimator(doorObj);
        SetMovement(false);  // Disable player input during this sequence
        isInteracting = true;
        GameObject npc = GameObject.Find("BagWoman");
        GameObject interactionCollider = GameObject.Find("InteractionCollider");
        interactionCollider.GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.2f);

        // Move player to the first target position
        Vector2 targetPosition = new Vector2(9.66f, -0.99f);
        while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
            animator.SetFloat("Vertical", 1);  // Moving up
            animator.SetFloat("Speed", 1);
            yield return null;
        }
        animator.SetFloat("Vertical", 0);  // Stop vertical movement
        animator.SetFloat("Speed", 0);

        // Trigger player and door opening animations
        

        GetComponent<AnimatorSynchroniser>().SetTrigger("openDoor");
        yield return new WaitForSeconds(0.6f);
        // Immediately move to the second target position
        Vector2 targetPosition2 = new Vector2(10.34f, -2.23f);
        StartCoroutine(MoveToPosition(rb, targetPosition2, moveSpeed ));

        // Wait for door open animation to complete before continuing
        float doorOpenDuration = doorAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(doorOpenDuration);

        // Once player has moved and door is opened, handle NPC interaction
        StartCoroutine(HandleNPCInteraction(npc));

        // Animation triggers for completing interaction
        animator.SetTrigger("doneInteracting");
        SetMovement(true);
        isInteracting = false;


    }

    private IEnumerator MoveToPosition(Rigidbody2D rb, Vector2 targetPosition, float speed)
    {
        {
            // Use a slightly bigger threshold for checking the distance to ensure reaching the target position
            while (Vector2.Distance(rb.position, targetPosition) > 0.01f)
            {
                Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed/2 * Time.fixedDeltaTime);
                rb.MovePosition(newPosition);
                yield return null;
            }
        }
    }

    private IEnumerator HandleNPCInteraction(GameObject npc)
    {
        Vector2 npctargetPosition = new Vector2(9.99f, -0.77f);
        while (Vector2.Distance(npc.transform.position, npctargetPosition) > 0.1f)
        {
            Vector2 newPositionNPC = Vector2.MoveTowards(npc.GetComponent<Rigidbody2D>().position, npctargetPosition, moveSpeed / 2 * Time.fixedDeltaTime);
            npc.GetComponent<Rigidbody2D>().MovePosition(newPositionNPC);
            npc.GetComponent<Animator>().SetTrigger("start_walk");
            yield return null;
        }

        StartCoroutine(FadeOutAndDestroy(npc));
    }

    private IEnumerator FadeOutAndDestroy(GameObject npc)
    {
        SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();
        float fadeDuration = 1.0f;
        float fadeRate = 1.0f / fadeDuration;

        for (float i = 1; i >= 0; i -= Time.deltaTime * fadeRate)
        {
            Color newColor = spriteRenderer.color;
            newColor.a = i;
            spriteRenderer.color = newColor;
            yield return null;
        }

        Destroy(npc);
    }
}