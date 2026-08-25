using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : CoreService
{
    [SerializeField] private Player playerPrefab;

    public override UniTask Initialize()
    {
        Player player = Instantiate(playerPrefab);
        player.name = playerPrefab.name;
        DontDestroyOnLoad(player.gameObject);

        SceneManager.sceneLoaded += PlaceInScene;

        return UniTask.CompletedTask;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= PlaceInScene;
    }

    private void PlaceInScene(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        Vector3? savedPosition = Player.instance.GetSavedScenePosition();
        Player.instance.transform.position = savedPosition ?? PlayerSpawnPoint.Instance.transform.position;
    }
}
