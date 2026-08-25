using Cysharp.Threading.Tasks;
using UnityEngine;

public class FrameRateManager : CoreService
{
    [SerializeField] private int targetFPS = 120;

    public override UniTask Initialize()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;

        return UniTask.CompletedTask;
    }
}
