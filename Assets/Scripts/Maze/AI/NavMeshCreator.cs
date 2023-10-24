using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshCreator : MonoBehaviour
{
    public void ResizeNavMesh(float scaleX, float scaleY)
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
        transform.position = new Vector3(scaleX / 2 - 2, scaleY / 2 - 2);
    }
}
