using UnityEngine;

public class SoundsHolder : MonoBehaviour
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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        SoundManager.Initialize();
    }
}
