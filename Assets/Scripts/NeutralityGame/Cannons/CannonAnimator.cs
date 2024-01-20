using System.Collections;
using UnityEngine;

public class CannonAnimator : MonoBehaviour
{
    private Animator m_Animator;
    private float animationDuration = 0.4f;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public IEnumerator PlayShootAnimation()
    {
        m_Animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(animationDuration);
    }
}
