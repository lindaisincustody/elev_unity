using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    private bool isLockPicked = false;
    private bool isPinAligned = false;
    private HollowCircle collidedHollowCircle;

    [SerializeField]
    private HollowCircleManager mngr;
    private bool shouldResetLevel = true;
    private bool isResetting = false;

    void Update()
    {
        if (!isLockPicked && !isResetting) // Check if not resetting.
        {
            gameObject.transform.Rotate(0f, 0f, -15f * Time.deltaTime * 10f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (HasCollidedWithHollowCircle())
                {
                    HitHollowCircle();
                }
                else if (shouldResetLevel) 
                {
                    
                    isResetting = true; 
                    mngr.MissAnimation();
                    ResetGameToLevel1();
                }
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
        Debug.Log("Hit");
        if (collidedHollowCircle != null)
        {
            collidedHollowCircle.HitHollowCircle();
        }
    }

    void ResetGameToLevel1()
    {

        isResetting = true;

        StartCoroutine(ResetAndResume());
    }

    private IEnumerator ResetAndResume()
    {
        yield return new WaitForSeconds(1f); 

        isResetting = false; // Resetting is complete.
        mngr.ResetGameToLevel1();
    }

    public void SetShouldResetLevel(bool shouldReset)
    {
        shouldResetLevel = shouldReset;
    }
}