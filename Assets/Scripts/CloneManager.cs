using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    public GameObject playerPrefab; // Assign the player prefab
    public Transform playerTransform; // Assign the player's transform
    public Animator playerAnimator; // Player's animator
    public Sprite cloneSprite; // Sprite for the clones, set this in the inspector

    private List<Vector2> relativePositions = new List<Vector2> {
        new Vector2(0, 1), // Up
        new Vector2(1, 0), // Right
        new Vector2(0, -1), // Down
        new Vector2(-1, 0)  // Left
    };

    public List<GameObject> clones = new List<GameObject>();

    private void Start()
    {
        playerAnimator = playerTransform.GetComponent<Animator>();
    }

    public void SpawnClones()
    {
        foreach (Vector2 pos in relativePositions)
        {
            GameObject clone = Instantiate(playerPrefab, (Vector2)playerTransform.position + pos, Quaternion.identity);

            // Disable the player movement script to prevent direct control.
            clone.GetComponent<PlayerMovement>().enabled = false;

            // Change Rigidbody2D to Kinematic to prevent physical interactions.
            Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();
            if (cloneRb != null)
            {
                cloneRb.bodyType = RigidbodyType2D.Kinematic;
            }

            // Optionally, adjust the collider to prevent physical interactions.
            Collider2D cloneCollider = clone.GetComponent<Collider2D>();
            if (cloneCollider != null)
            {
                cloneCollider.isTrigger = true;
            }

            clones.Add(clone);
        }
    }

    void Update()
    {
        if (clones.Count > 0)
        {
            UpdateClones();
        }
    }

    public void UpdateClones()
    {
        for (int i = 0; i < clones.Count; i++)
        {
            clones[i].transform.position = (Vector2)playerTransform.position + relativePositions[i];
            Animator cloneAnimator = clones[i].GetComponent<Animator>();
            // Copy the animation state from the player to the clone
            cloneAnimator.SetFloat("Horizontal", playerAnimator.GetFloat("Horizontal"));
            cloneAnimator.SetFloat("Vertical", playerAnimator.GetFloat("Vertical"));
            cloneAnimator.SetFloat("Speed", playerAnimator.GetFloat("Speed"));
        }
    }

    public void ClearClones()
    {
        foreach (var clone in clones)
        {
            Destroy(clone);
        }
        clones.Clear();
    }
}
