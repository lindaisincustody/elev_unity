using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutGameAmbient : MonoBehaviour
{
    private void Start()
    {
        SoundManager.PlayAmbientSound(SoundManager.Sound.ShopBackground);
    }
}
