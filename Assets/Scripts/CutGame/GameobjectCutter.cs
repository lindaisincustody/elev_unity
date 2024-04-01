using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectCutter : MonoBehaviour
{
    public Cutter cutter;
    public Transform cutterPointA;
    public Transform cutterPointB;
    Vector3 pointA;
    Vector3 pointB;
    Camera cam;

    System.Action onCut;
    private bool canCut = false;

    void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    public void Init(System.Action cutAction)
    {
        onCut = cutAction;
    }

    private void Update()
    {
        if (!canCut)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            CreateSlicePlane();
    }

    void CreateSlicePlane()
    {
        pointA = cutterPointA.transform.position;
        pointB = cutterPointB.transform.position;
        Vector3 pointInPlane = (pointA + pointB) / 2;

        Vector3 cutPlaneNormal = Vector3.Cross((pointA - pointB), (pointA - cam.transform.position)).normalized;
        Quaternion orientation = Quaternion.FromToRotation(Vector3.up, cutPlaneNormal);

        var all = Physics.OverlapBox(pointInPlane, new Vector3(8, 0.3f, 1), orientation);
        
        foreach (var hit in all)
        {
            ObjectToCut filter = hit.gameObject.GetComponent<ObjectToCut>();
            if (filter != null && filter.Cut() != null)
            {
                cutter.Cut(hit.gameObject, pointInPlane, cutPlaneNormal);   
                break;
            }
        }

        onCut?.Invoke();
        canCut = false;
    }

    public void EnableCutting()
    {
        canCut = true;
    }
}
