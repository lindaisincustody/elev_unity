using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingStyleChanger : MonoBehaviour
{
    [SerializeField] private LetterDrawing.DrawingMode drawingStyle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Entity>().Get<LetterDrawing>().ChangeState(drawingStyle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Entity>().Get<LetterDrawing>().RevertToPreviousState();
        }
    }
}
