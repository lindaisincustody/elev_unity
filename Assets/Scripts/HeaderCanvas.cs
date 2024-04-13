using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaderCanvas : MonoBehaviour
{
    public static HeaderCanvas instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
