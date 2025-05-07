using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : Component
{
    [SerializeField] private TrailRenderer trail;

    public void EnableTrail()
    {
        trail.Clear();
        trail.enabled = true;
    }    

    public void DisableTrail()
    {
        trail.enabled = false;
    }
}
