using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelligencePlayerController : MonoBehaviour
{
    public Sprite xSprite;
    public Sprite oSprite;

    GameController game;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other)
    {
        GameController gameCon = FindObjectOfType<GameController>();

        if (other.gameObject.CompareTag("CorrectDestination"))
            gameCon.CheckWinCondition();

        if (other.CompareTag("X"))
        {
            Debug.Log("X");
            // Player stepped on X, change it to O
            ChangeObjectSpriteAndTag(other.gameObject, oSprite, "O");
        }
        else if (other.CompareTag("O"))
        {
            Debug.Log("O");
            // Player stepped on O, change it to X
            ChangeObjectSpriteAndTag(other.gameObject, xSprite, "X");
        }
    }

    void ChangeObjectSpriteAndTag(GameObject obj, Sprite newSprite, string newTag)
    {
        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        // Check if the SpriteRenderer component is not null
        if (spriteRenderer != null)
        {
            // Set the sprite and tag based on the provided parameters
            spriteRenderer.sprite = newSprite;
            obj.tag = newTag;

            // Print the tag for debugging
            Debug.Log("Object tag: " + obj.tag);
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on object: " + obj.name);
        }
    }
    void Update()
    {
        Debug.Log(GetCurrentPosition());
    }
    public Vector2Int GetCurrentPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        
    }
    public void TeleportPlayer(Vector2Int position)
    {
        // Set the player's position to the specified position
        transform.position = new Vector3(position.x, position.y, 0);
    }
}