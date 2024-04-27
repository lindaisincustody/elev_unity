using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    private bool isPinAligned = false;
    public bool isActive = false;
    private HollowCircle collidedHollowCircle;

    [SerializeField]
    private HollowCircleManager mngr;

    void Update()
    {
        if (!isActive)
            return;

        gameObject.transform.Rotate(0f, 0f, -15f * Time.deltaTime * 10f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (HasCollidedWithHollowCircle())
            {
                HitHollowCircle();
            }
            else
            {
                mngr.MissAnimation();
            }
        }
    }

    private bool HasCollidedWithHollowCircle()
    {
        return isPinAligned && collidedHollowCircle != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HollowCircle"))
        {
            isPinAligned = true;
            collidedHollowCircle = other.GetComponent<HollowCircle>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HollowCircle"))
        {
            isPinAligned = false;
            collidedHollowCircle = null;
        }
    }

    void HitHollowCircle()
    {
        if (collidedHollowCircle != null)
        {
            collidedHollowCircle.HitHollowCircle();
        }
    }
}