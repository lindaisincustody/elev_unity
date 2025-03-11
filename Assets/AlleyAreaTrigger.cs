using UnityEngine;

public class AlleyAreaTrigger : MonoBehaviour
{
    public CameraBoundsSwitcher boundsSwitcher; // assign in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boundsSwitcher.SwitchToAlleyBounds();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boundsSwitcher.SwitchToDefaultBounds();
        }
    }
}