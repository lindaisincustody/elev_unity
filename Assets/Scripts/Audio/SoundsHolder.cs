using Cysharp.Threading.Tasks;
using UnityEngine;

public class SoundsHolder : CoreService
{
    // Singleton instance
    private static SoundsHolder instance;

    [SerializeField]
    private Sounds sounds;

    public static Sounds Sounds => Instance.sounds;

    private static SoundsHolder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundsHolder>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("SoundsHolder");
                    instance = obj.AddComponent<SoundsHolder>();
                }
            }
            return instance;
        }
    }

    public override UniTask Initialize()
    {
        SoundManager.Initialize();

        return UniTask.CompletedTask;
    }
}
