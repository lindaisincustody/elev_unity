using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesAnimation : MonoBehaviour
{
    [SerializeField] Animation leftAnim;
    [SerializeField] Animation rightAnim;

    public void Blink()
    {
        leftAnim.Play();
        rightAnim.Play();
    }
}
