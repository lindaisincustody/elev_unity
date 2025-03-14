using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.instance.InDangerZone = true;
            if (!Player.instance.InSafeZone)
                Player.instance.transform.position = Player.instance.GetSavedScenePosition().Value;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.instance.InDangerZone = false;
        }
    }
}
