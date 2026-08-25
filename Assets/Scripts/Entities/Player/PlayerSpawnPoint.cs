using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public static PlayerSpawnPoint Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
