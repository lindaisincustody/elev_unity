using UnityEngine;

public class DrawnPathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Feet"))
        {
            Player.instance.InSafeZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Feet"))
        {
            Player.instance.InSafeZone = false;
            if (Player.instance.InDangerZone)
                Player.instance.transform.position = Player.instance.GetSavedScenePosition().Value;
        }
    }
}
