using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [SerializeField] private int targetFPS = 120;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }
}