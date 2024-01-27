using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalesWeight : MonoBehaviour
{
    [SerializeField] private Transform leftScaleSide;
    [SerializeField] private Transform rightScaleSide;

    public float yValue;

    // Update is called once per frame
    void Update()
    {
        if (leftScaleSide.position.y < yValue && rightScaleSide.position.y < yValue)
        {
            Debug.Log("WIn");
        }
    }
}
