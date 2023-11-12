using PluginsEngine.FileAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    public static int sceneToLoad = 2;
    public bool waitForSavedataLoad = false;


    private void Awake()
    {
        Debug.Log("||||||||||||||||||||||||||||||||||| PRELOAD ||||||||||||||||||||||||||||||||||||||||");
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadNewScene());
    }


    private IEnumerator LoadNewScene()
    {
        if (waitForSavedataLoad)
        {
            SaveManager saveManager = GetComponent<SaveManager>();
            Debug.Log("Waiting for Savegame");
            yield return new WaitUntil(() => saveManager.loadedAndInitialized);

            //UniPrefs.AutoSave = true;
            Debug.Log("Savegame loaded and LevelData initialized");
        }
        if (sceneToLoad != -1)
        {

            yield return new WaitUntil(() => SceneManager.sceneCount == 1);

            if (sceneToLoad == 1)
            {
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
            }
        }
    }
}
