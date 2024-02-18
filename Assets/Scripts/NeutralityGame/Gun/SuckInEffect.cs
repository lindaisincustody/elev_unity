using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckInEffect : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] Transform gun;
    Transform softbody;
    [SerializeField] Transform[] gunEndpoints;
    [SerializeField] private GameObject[] colliders;

    bool isSucking = false;
    bool isWindingUp = false;

    private void Start()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (softbody == null && isSucking)
        {
            isSucking = false;
        }
        if (isSucking)
        {
            lineRenderer.SetPosition(0, gun.position);
            for (int i = 1; i < gunEndpoints.Length + 1; i++)
            {
                lineRenderer.SetPosition(i, softbody.position);
            }
        }
        else if (isWindingUp)
        {
            lineRenderer.SetPosition(0, gun.position);
            for (int i = 1; i < gunEndpoints.Length + 1; i++)
            {
                lineRenderer.SetPosition(i, gunEndpoints[i - 1].position);
            }
            UpdateColliders();
        }
    }

    public void ActivateSuckInEffect(Transform newSoftbody)
    {
        softbody = newSoftbody;
        isSucking = true;
        lineRenderer.enabled = true;
        foreach (var collider in colliders)
        {
            collider.SetActive(true);
        }
        StartCoroutine(StartSucking());
    }

    public void ActivateWind()
    {
        ActivateSuckInColliders();
        lineRenderer.enabled = true;
        isWindingUp = true;
    }

    private void UpdateColliders()
    {
        for (int i = 0; i < gunEndpoints.Length - 1; i++)
        {
            // Calculate the position and size of the collider between each pair of adjacent points
            Vector3 position = (gunEndpoints[i].position + gunEndpoints[i + 1].position) / 2f;
            float distance = Vector3.Distance(gunEndpoints[i].position, gunEndpoints[i + 1].position);
            float colliderWidth = 0.1f; // Adjust as needed
            float colliderHeight = distance;
            float colliderDepth = 0.1f; // Adjust as needed

            // Update collider's position and size
            colliders[i].transform.position = position;
            colliders[i].transform.localScale = new Vector3(colliderWidth, colliderHeight, colliderDepth);

            // Rotate collider to match the direction of the line segment
            Vector3 direction = gunEndpoints[i + 1].position - gunEndpoints[i].position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            colliders[i].transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }


    private IEnumerator StartSucking()
    {
        while (true)
        {
            if (softbody == null)
            {
                isSucking = false;
                lineRenderer.enabled = false;
                foreach (var collider in colliders)
                {
                    collider.SetActive(false);
                }
                break;
            }
            if (Vector3.Distance(softbody.position, gun.position) <= 1f)    
            {
                isSucking = false;
                lineRenderer.enabled = false;
                foreach (var collider in colliders)
                {
                    collider.SetActive(false);
                }
                break;
            }
            yield return null;
        }
    }
    
    public void ActivateSuckInColliders()
    {
        foreach (var collider in colliders)
        {
            collider.SetActive(true);
        }
    }
}
