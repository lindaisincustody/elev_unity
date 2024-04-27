using UnityEngine;

public class AnimatorSynchroniser : MonoBehaviour
{
    public Animator[] animators;

    private void Start()
    {
        // Initializes the animators array with all child Animator components
        // You can still manually assign animators in the inspector if this line is removed
        animators = GetComponentsInChildren<Animator>();
    }

    // Method to add an Animator from another GameObject
    public void AddAnimator(GameObject obj)
    {
        Animator newAnimator = obj.GetComponent<Animator>();
        if (newAnimator != null)
        {
            // Create a new array with one extra slot
            Animator[] newAnimators = new Animator[animators.Length + 1];
            animators.CopyTo(newAnimators, 0);
            newAnimators[animators.Length] = newAnimator;
            animators = newAnimators;
        }
        else
        {
            Debug.LogError("No Animator found on the object: " + obj.name);
        }
    }

    public void SetBool(string name, bool value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetBool(name, value);
        }
    }

    public void SetFloat(string name, float value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetFloat(name, value);
        }
    }

    public void SetInteger(string name, int value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetInteger(name, value);
        }
    }

    public void SetTrigger(string name)
    {
        foreach (Animator anim in animators)
        {
            anim.SetTrigger(name);
        }
    }

    public bool GetBool(string name)
    {
        return animators[0].GetBool(name);
    }

    public float GetFloat(string name)
    {
        return animators[0].GetFloat(name);
    }

    public int GetInteger(string name)
    {
        return animators[0].GetInteger(name);
    }
}
