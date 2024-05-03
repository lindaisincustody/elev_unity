using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationAmbientPlayer : MonoBehaviour
{
    private void Start()
    {
        SoundManager.PlayAmbientSound(SoundManager.Sound.TrainStationAmbient);
    }
}
