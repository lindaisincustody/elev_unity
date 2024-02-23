using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
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

    public IEnumerator LoadScene(string sceneName)
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
        transitionAnim.SetTrigger("Start");
    }
    public IEnumerator LoadInScene(float x, float y)
    {
        PlayerMovement playerController = FindObjectOfType<PlayerMovement>();
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);

        if (playerController != null)
            playerController.transform.position = new Vector3(x, y, playerController.transform.position.z);

        transitionAnim.SetTrigger("Start");
    }
}
