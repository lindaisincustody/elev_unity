using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckInEffect : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] Transform gun;
    Transform softbody;

    bool isSucking = false;

    private void Start()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (!isSucking)
            return;
        lineRenderer.SetPosition(0, gun.position);
        lineRenderer.SetPosition(1, softbody.position);
    }

    public void ActivateSuckInEffect(Transform newSoftbody)
    {
        isSucking = true;
        softbody = newSoftbody;
        lineRenderer.enabled = true;
        StartCoroutine(StartSucking());
    }

    private IEnumerator StartSucking()
    {
        while (true)
        {
            if (Vector3.Distance(softbody.position, gun.position) <= 1f)    
            {
                isSucking = false;
                lineRenderer.enabled = false;
                break;
            }
            yield return null;
        }
    }
}
