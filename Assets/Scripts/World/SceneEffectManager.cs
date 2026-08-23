using UnityEngine;
using UnityEngine.Rendering;

public class SceneEffectManager : MonoBehaviour
{
    [field: SerializeField] public Volume GlobalVolume { get; private set; }

    public static SceneEffectManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Double SceneEffectManager or singleton problem");

        Instance = this;
    }
}
