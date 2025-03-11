using UnityEngine;
using Cinemachine;

public class CameraBoundsSwitcher : MonoBehaviour
{
    public CinemachineConfiner2D confiner;
    public Collider2D alleyBounds;
    public Collider2D defaultBounds;

    // Call this when teleporting to the alley.
    public void SwitchToAlleyBounds()
    {
        confiner.m_BoundingShape2D = alleyBounds;
    }

    // Call this when leaving the alley.
    public void SwitchToDefaultBounds()
    {
        confiner.m_BoundingShape2D = defaultBounds;
    }
}