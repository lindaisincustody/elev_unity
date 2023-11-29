using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    public ScaleLogic scaleLogic;
    public bucketType bucket;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Consumable"))
        {
            if (bucket == bucketType.Right)
                scaleLogic.AddWeightToRightBucket();
            if (bucket == bucketType.Left)
                scaleLogic.AddWeightToLeftBucket();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Consumable"))
        {
            if (bucket == bucketType.Right)
                scaleLogic.RemoveWeightFromRightBucket();
            if (bucket == bucketType.Left)
                scaleLogic.RemoveWeightFromLeftBucket();
        }
    }
}

public enum bucketType
{
    Left,
    Right
}
