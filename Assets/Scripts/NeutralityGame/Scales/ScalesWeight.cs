using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalesWeight : MonoBehaviour
{
    [SerializeField] ScaleScoreVisualizer leftScaleVisualizer;
    [SerializeField] ScaleScoreVisualizer rightScaleVisualizer;

    List<int> leftScaleObjects = new List<int>();
    List<int> rightScaleObjects = new List<int>();

    public void AddToRightScale(int newObject)
    {
        if (rightScaleObjects.Contains(newObject))
            return;

        rightScaleObjects.Add(newObject);
        rightScaleVisualizer.UpdateVisualizer(rightScaleObjects.Count, leftScaleObjects.Count);
    }

    public void AddToLeftScale(int newObject)
    {
        if (leftScaleObjects.Contains(newObject))
            return;

        leftScaleObjects.Add(newObject);
        leftScaleVisualizer.UpdateVisualizer(leftScaleObjects.Count, rightScaleObjects.Count);
    }

    public void RemoveFromRightScale(int removedObject)
    {
        if (!rightScaleObjects.Contains(removedObject))
            return;

        rightScaleObjects.Remove(removedObject);
        rightScaleVisualizer.UpdateVisualizer(rightScaleObjects.Count, leftScaleObjects.Count);
    }

    public void RemoveFromLeftScale(int removedObject)
    {
        if (!leftScaleObjects.Contains(removedObject))
            return;

        leftScaleObjects.Remove(removedObject);
        leftScaleVisualizer.UpdateVisualizer(leftScaleObjects.Count, rightScaleObjects.Count);
    }
}
