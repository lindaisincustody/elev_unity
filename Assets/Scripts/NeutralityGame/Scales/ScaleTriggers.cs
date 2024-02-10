using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTriggers : MonoBehaviour
{
    [SerializeField] ScalesWeight scales;
    [SerializeField] Scales scaleSide;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Consumable"))
        {
            if (scaleSide == Scales.Left)
                scales.AddToLeftScale(collision.GetInstanceID());
            else
                scales.AddToRightScale(collision.GetInstanceID());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Consumable"))
        {
            if (scaleSide == Scales.Left)
                scales.RemoveFromLeftScale(collision.GetInstanceID());
            else
                scales.RemoveFromRightScale(collision.GetInstanceID());
        }
    }

    public enum Scales { Left, Right}
}
