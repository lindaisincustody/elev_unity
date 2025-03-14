using UnityEngine;
using Cinemachine;

public class CameraBoundsSwitcher : MonoBehaviour
{
    public CinemachineConfiner2D confiner;
    public Collider2D alleyBounds;
    public Collider2D defaultBounds;

    public void SwitchToAlleyBounds()
    {
        confiner.m_BoundingShape2D = alleyBounds;
    }

    public void SwitchToDefaultBounds()
    {
        confiner.m_BoundingShape2D = defaultBounds;
    }
}