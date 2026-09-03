using UnityEngine;

public class DrawingStyleChanger : MonoBehaviour
{
    [SerializeField] private DrawingMode drawingStyle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Entity>().Get<LetterDrawing>().PushZoneMode(this, drawingStyle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Entity>().Get<LetterDrawing>().PopZoneMode(this);
        }
    }
}
