using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JelloPSContorller
{
    private ParticleSystem.MainModule mainModule;

    public void ChangeParticleColor(ParticleSystem ps, Color color)
    {
        mainModule = ps.main;
        mainModule.startColor = color;
    }
}
