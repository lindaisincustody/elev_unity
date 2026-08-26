using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameSession : CoreService
{
    public static GameSession Instance { get; private set; }

    public bool IsRunning { get; private set; }

    public event Action OnGameStarted;
    public event Action OnGameEnded;

    public override UniTask Initialize()
    {
        Instance = this;

        SceneManager.sceneLoaded += HandleSceneLoaded;

        if (SceneManager.GetActiveScene().isLoaded)
            RefreshFromActiveScene();

        return UniTask.CompletedTask;
    }

    private void HandleSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        RefreshFromActiveScene();
    }

    private void RefreshFromActiveScene()
    {
        SetRunning(SceneManager.GetActiveScene().name != Constants.SceneNames.MainMenu);
    }

    private void SetRunning(bool running)
    {
        if (running == IsRunning)
            return;

        IsRunning = running;

        if (running)
            OnGameStarted?.Invoke();
        else
            OnGameEnded?.Invoke();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }
}
