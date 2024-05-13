using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private DataManager dataManager;
    public static SceneController instance;
    [SerializeField] Animator transitionAnim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dataManager = DataManager.Instance;
    }

    public IEnumerator LoadScene(string sceneName)
    {
        transitionAnim.SetTrigger("End");
        if (Player.instance != null)
        {
            Player.instance.SaveCurrentScenePosition();
            DataManager.Instance.SavePillTime();
        }
        DataManager.Instance.SaveScene(sceneName);

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
        transitionAnim.SetTrigger("Start");
    }
    public IEnumerator LoadInScene(float x, float y)
    {
        PlayerMovement playerController = FindObjectOfType<PlayerMovement>();
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2f);

        if (playerController != null)
            playerController.transform.position = new Vector3(x, y, playerController.transform.position.z);
        if (Player.instance != null)
            Player.instance.SaveCurrentScenePosition();
        transitionAnim.SetTrigger("Start");
    }
}
