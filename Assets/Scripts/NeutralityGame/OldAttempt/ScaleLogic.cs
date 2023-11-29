using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLogic : MonoBehaviour
{
    [SerializeField] private Transform beam;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float angleByWeight = 10;

    private int RightSideWeight = 0;
    private int LeftSideWeight = 0;
    private Quaternion targetRotation = Quaternion.identity;

    private void Update()
    {
        beam.rotation = Quaternion.Slerp(beam.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void UpdateBeamAngle()
    {
        float angle = (LeftSideWeight - RightSideWeight) * angleByWeight;
        targetRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void AddWeightToRightBucket()
    {
        RightSideWeight++;
        UpdateBeamAngle();
    }

    public void AddWeightToLeftBucket()
    {
        LeftSideWeight++;
        UpdateBeamAngle();
    }

    public void RemoveWeightFromRightBucket()
    {
        RightSideWeight--;
        UpdateBeamAngle();
    }

    public void RemoveWeightFromLeftBucket()
    {
        LeftSideWeight--;
        UpdateBeamAngle();
    }
}
