using Cysharp.Threading.Tasks;
using UnityEngine;

public class ConfigManager : CoreService
{
    public static ConfigManager Instance { get; private set; }

    [field: SerializeField] public DrawingRigReferences DrawingRig { get; private set; }

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }
}
