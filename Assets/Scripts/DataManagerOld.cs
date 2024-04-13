using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManagerOld : MonoBehaviour
{
    [SerializeField] private Transform player;
    private SaveManager _saveManager;

    // Start is called before the first frame update
    void Start()
    {
        _saveManager = SaveManager.instance;
        string sceneName = SceneManager.GetActiveScene().name;
        if (_saveManager.saveGame.savedProgress.ContainsKey(sceneName))
        {
            Level levelData = _saveManager.saveGame.savedProgress[sceneName];
            Vector2 playerPosition = levelData.playerPos;
            SetPlayerPosition(playerPosition);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            SavePlayerPosition(() => { });
    }

    private void SetPlayerPosition(Vector2 pos)
    {
        player.position = pos;
    }

    public void SavePlayerPosition(Action onSaved)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (_saveManager.saveGame.savedProgress.ContainsKey(sceneName))
        {
            _saveManager.saveGame.savedProgress[sceneName] = new Level(sceneName, player.position);
        }
        else
        {
            _saveManager.saveGame.savedProgress.Add(sceneName, new Level(sceneName, player.position));
        }
        _saveManager.SaveAsync(exists =>
        {
            if (onSaved != null)
                onSaved.Invoke();
        });
    }
}
