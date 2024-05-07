using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeightCalculator : MonoBehaviour
{
    [SerializeField] Scales leftScale;
    [SerializeField] Scales rightScale;    
    [SerializeField] Scales leftSingleWeight;
    [SerializeField] Scales rightSingleWeight;
    [SerializeField] int weightMultiplier = 10000;
    private MeshFilter leftMeshFilter;
    private MeshFilter rightMeshFilter;

    float totalLeftWeight = 0;
    float totalRightWeight = 0;



    public WeightSide SetMeshFilters(MeshFilter leftFilter, MeshFilter rightFilter)
    {
        leftMeshFilter = leftFilter;
        rightMeshFilter = rightFilter;
        return CalculateWeights();
    }

    public WeightSide CalculateWeights()
    {
        float leftWeight; float rightWeight;
        // right = switch
        WeightSide switchSides = WeightSide.Left;
        leftWeight = CalculateMeshFilterArea(leftMeshFilter);
        rightWeight = CalculateMeshFilterArea(rightMeshFilter);

        if (leftWeight >= rightWeight)
        {
            if (totalLeftWeight < totalRightWeight)
            {
                UpdateText(leftScale, leftSingleWeight, WeightSide.Left, leftWeight);
                UpdateText(rightScale, rightSingleWeight, WeightSide.Right, rightWeight);
            }
            else
            {
                switchSides = WeightSide.Right;
                UpdateText(leftScale, leftSingleWeight, WeightSide.Left, rightWeight);
                UpdateText(rightScale, rightSingleWeight, WeightSide.Right, leftWeight);
            }
        }
        else
        {
            if (totalLeftWeight < totalRightWeight)
            {
                switchSides = WeightSide.Right;
                UpdateText(leftScale, leftSingleWeight, WeightSide.Left, rightWeight);
                UpdateText(rightScale, rightSingleWeight, WeightSide.Right, leftWeight);
            }
            else
            {
                UpdateText(leftScale, leftSingleWeight, WeightSide.Left, leftWeight);
                UpdateText(rightScale, rightSingleWeight, WeightSide.Right, rightWeight);
            }
        }
           

        return switchSides;
    }

    float CalculateMeshFilterArea(MeshFilter meshFilter)
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        float totalArea = 0f;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 v1 = vertices[mesh.triangles[i]];
            Vector3 v2 = vertices[mesh.triangles[i + 1]];
            Vector3 v3 = vertices[mesh.triangles[i + 2]];

            totalArea += TriangleArea(v1, v2, v3);
        }

        return totalArea;
    }

    void UpdateText(Scales textHolder, Scales singleScaleText, WeightSide side, float totalArea)
    {
        if (side == WeightSide.Left)
        {
            totalLeftWeight += totalArea;
            singleScaleText.UpdateScales(totalArea * weightMultiplier);
            textHolder.UpdateScales((totalLeftWeight * weightMultiplier));
        }
        else
        {
            totalRightWeight += totalArea;
            singleScaleText.UpdateScales(totalArea * weightMultiplier);
            textHolder.UpdateScales((totalRightWeight * weightMultiplier));
        }
    }

    float TriangleArea(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        // Calculate the area of a triangle given its vertices
        return Mathf.Abs(Vector3.Cross(v2 - v1, v3 - v1).magnitude) / 2f;
    }

    public float GetWeightDifference()
    {
        return Mathf.Abs(totalLeftWeight * weightMultiplier - totalRightWeight * weightMultiplier);
    }

    public void SetWeightDifference(float x)
    {
        totalLeftWeight = x;
        totalRightWeight = x;
    }
}

public enum WeightSide
{
    Left, Right
}
