using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToCut : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    bool isCut = false;

    public MeshFilter Cut()
    {
        if (isCut)
            return null;
        else
        {
            isCut = true;
            return meshFilter;
        }
    }

    public void SetAsCut()
    {
        meshFilter = GetComponent<MeshFilter>();
        isCut = true;
    }
}
