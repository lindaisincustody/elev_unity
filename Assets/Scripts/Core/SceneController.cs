using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] Animator transitionAnim;

    public IEnumerator LoadScene(string sceneName)
    {
        transitionAnim.SetTrigger("End");
        if (Player.instance != null)
        {
            Player.instance.SaveCurrentScenePosition();
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
        transitionAnim.SetTrigger("Start");
    }
    public IEnumerator LoadInScene(float x, float y)
    {
        Player player = Player.instance;
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2f);

        player.transform.position = new Vector3(x, y, player.transform.position.z);
        PlayerFollowCamera.Instance.Snap();

        player.SaveCurrentScenePosition();

        transitionAnim.SetTrigger("Start");
    }
}
