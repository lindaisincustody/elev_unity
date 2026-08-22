using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    [field: SerializeField] public DrawingRigReferences DrawingRig { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
