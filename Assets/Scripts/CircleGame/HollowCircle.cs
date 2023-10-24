using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowCircle : MonoBehaviour
{

    private Animator animator;
    private HollowCircleManager manager;


    private void Start()
    {
        animator = GetComponent<Animator>();
      
        animator.enabled = false;
    }
    public void Initialize(HollowCircleManager manager)
    {
        this.manager = manager;
    }

    public void HitHollowCircle()
    {
        animator.enabled = true;
        manager.TwitchAnimation();
        animator.SetTrigger("Hollow_trigger");
        
        if (manager != null)
        {
            // Call after a delay of 0.5 seconds.
            Invoke("RemoveHollowCircleAfterDelay", 0.5f);
        }
    }

    private void RemoveHollowCircleAfterDelay()
    {
        if (manager != null)
        {
            manager.RemoveHollowCircle(gameObject);
        }
    }
}
