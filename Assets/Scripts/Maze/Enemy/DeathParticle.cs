using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2);   
    }
}
