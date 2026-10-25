using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : CoreService
{
    [SerializeField] private Player playerPrefab;

    public override UniTask Initialize()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        GameSession.Instance.OnGameStarted += SpawnPlayer;

        if (GameSession.Instance.IsRunning)
            SpawnPlayer();

        return UniTask.CompletedTask;
    }

    private void SpawnPlayer()
    {
        if (Player.instance == null)
        {
            Player player = Instantiate(playerPrefab);
            player.name = playerPrefab.name;
            DontDestroyOnLoad(player.gameObject);
        }

        PlaceInScene();
    }

    private void HandleSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (GameSession.Instance.IsRunning)
            PlaceInScene();
    }

    private void PlaceInScene()
    {
        Player.instance.transform.position = Player.instance.Get<PlayerScenePosition>().GetSpawnPosition();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        GameSession.Instance.OnGameStarted -= SpawnPlayer;
    }
}
