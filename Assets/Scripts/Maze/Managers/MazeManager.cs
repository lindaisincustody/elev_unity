using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MazeManager : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject loseText;
    private MiniGamesManager miniGamesManager;
    private float endScreenTime = 3f;

    private void Awake()
    {
        miniGamesManager = FindObjectOfType<MiniGamesManager>();
    }

    private void Start()
    {
        StartMaze();
    }

    public void Win()
    {
        Debug.Log("You Won!");
        var level = PlayerPrefs.GetInt(Constants.PlayerPrefs.CoordinationLevel, 1);
        level++;
        PlayerPrefs.SetInt(Constants.PlayerPrefs.CoordinationLevel, level);
        miniGamesManager.CoordinationLevel++;
        SceneManager.LoadScene(Constants.SceneNames.MainScene);
    }

    public void Lose()
    {
        Debug.Log("You lost!");
        loseText.SetActive(true);
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(endScreenTime);
        SceneManager.LoadScene(Constants.SceneNames.MainScene);
    }

    private void StartMaze()
    {
        int level = PlayerPrefs.GetInt(Constants.PlayerPrefs.CoordinationLevel);
        mazeGenerator.StartGeneratingMaze(level + 5, level + 5);
    }
}
