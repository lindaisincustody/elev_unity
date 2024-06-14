using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MazeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] MazeScreen loseScreen;
    [SerializeField] MazeWinScreen winScreen;
    [SerializeField] MazePlayerMovement playerMovement;
    [SerializeField] CircleCollider2D playerCollider;

    [Header("Item references")]
    [SerializeField] PathShowersSpawner pathShowersSpawner;

    DataManager dataManager;

    private void Start()
    {
        dataManager = DataManager.Instance;
    }

    public void Win()
    {
        dataManager.AddLevel(Attribute.Coordination);
        dataManager.AddGold((int)(4 * dataManager.GetPlayerData().heroCoordination));
        playerCollider.enabled = false;
        playerMovement.StopPlayer();
        winScreen.ShowEndScreen();
    }

    public void Lose()
    {
        mazeGenerator.HideAllEnemies();
        playerCollider.enabled = false;
        playerMovement.StopPlayer();
        loseScreen.ShowEndScreen();
    }

    public void StartMaze()
    {
        SoundManager.PlayAmbientSound(SoundManager.Sound.HeartBeat, false, 0.5f);
        int level = dataManager.GetLevel(Attribute.Coordination);
        mazeGenerator.StartGeneratingMaze(level + 5, level + 5);
    }

    // Item methods
    public void IncreaseLuck(float luckIncrease)
    {
        Debug.Log("Luck increase byL " + luckIncrease);
    }

    public void IncreasePathShowerAmount(int additionalAmount)
    {
        pathShowersSpawner.IncreasePathShowerCount(additionalAmount);
    }
}
