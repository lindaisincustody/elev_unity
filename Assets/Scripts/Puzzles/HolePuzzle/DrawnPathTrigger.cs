using UnityEngine;

public class DrawnPathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Feet"))
        {
            Player.instance.Get<PlayerFooting>().SetOnPath(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Feet"))
        {
            Player.instance.Get<PlayerFooting>().SetOnPath(false);
        }
    }
}
