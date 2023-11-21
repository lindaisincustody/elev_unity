using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelligencePlayerController : MonoBehaviour
{
    public Sprite xSprite;
    public Sprite oSprite;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("X"))
        {
            // Player stepped on X, change it to O
            ChangeObjectSpriteAndTag(other.gameObject, oSprite, "O");
        }
        else if (other.CompareTag("O"))
        {
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
}
