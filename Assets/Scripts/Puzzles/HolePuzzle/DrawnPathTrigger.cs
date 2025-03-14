using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawnPathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.instance.InSafeZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.instance.InSafeZone = false;
            if (Player.instance.InDangerZone)
                Player.instance.transform.position = Player.instance.GetSavedScenePosition().Value;
        }
    }
}
