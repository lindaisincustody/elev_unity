using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PluginsEngine.FileAccess;


public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public SaveGame saveGame;
    public bool loaded;
    public bool loadedAndInitialized;
    public List<LevelData> allLevelData;

    private IAsyncPlugin _asyncPlugin
    {
        get
        {
            return FileAccessorFactory.GetAsyncPlugin;
        }
    }

    public string FullPath
    {
        get
        {
            return Application.persistentDataPath;
        }
    }

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.Log("ERROR!!!! SaveManager is already instanced!");
        }
    }

    private void Start()
    {
        if (saveGame)
        {
            saveGame = GetComponent<SaveGame>();
        }
        LoadAsync(exist =>
        {/*
            if (saveGame.savedProgress.Count < 1)
            {
                // If no save exists, save default
               // saveGame.savedProgress.Add("Floor1", new Level("Floor1", new Vector2(0, 0)));
               // SaveAsync(exists => { });
            }*/
        });
    }

    public void SaveAsync(Action<bool> callback)
    {
        if (saveGame)
        {
            try
            {
                _asyncPlugin.Save(FullPath + "/SaveData.json", ToBytes(JsonUtility.ToJson(saveGame, true)), callback);
            }
            catch (IOException)
            {
                Debug.Log("FA - Couldn't write Save File!");
            }
        }
        Debug.Log("SAVESYSTEM - FILEACCESS - Saved Game to " + "/SaveData.json");
    }

    public void LoadAsync(Action<bool> callback = null)
    {
        _asyncPlugin.FileExists(FullPath + "/SaveData.json", exist =>
        {
            if (exist)
            {
                _asyncPlugin.Load(FullPath + "/SaveData.json", data =>
                {
                    JsonUtility.FromJsonOverwrite(ToText(data), saveGame);
                    Debug.Log("FA - Loaded Game from " + "/SaveData.json");
                    InitializeLevelData();
                    callback?.Invoke(true);
                });
            }
            else
            {
                Debug.Log("SAVESYSTEM - FILEACCESS - No Savegame yet");
                // Set variables for game startup even if no savegame exists(it will be initialized later) - just a precaution. Tested.
                loaded = true;
                loadedAndInitialized = true;
                callback?.Invoke(false);
            }
        });
    }

    public void DeleteAsync()
    {
        _asyncPlugin.FileExists(FullPath + "/SaveData.json", exist =>
        {
            if (exist)
            {
                _asyncPlugin.FileDelete(FullPath + "/SaveData.json");
            }
            else
            {
                Debug.Log("SAVESYSTEM - DELETEION - No Savegame yet");
            }
        });
    }

    public void InitializeLevelData()
    {
        for (var index = 0; index < allLevelData.Count; index++)
        {
            var levelData = allLevelData[index];
            if (saveGame.savedProgress.ContainsKey(levelData.levelId))
            {
                Level currentSavedLvl = saveGame.savedProgress[levelData.levelId];
                levelData.loadedSaveData = currentSavedLvl;
            }
            levelData.isLoaded = true;
        }
        loadedAndInitialized = true;
    }

    private string ToText(byte[] data)
    {
        return System.Text.Encoding.UTF8.GetString(data);
    }

    private byte[] ToBytes(string data)
    {
        return System.Text.Encoding.UTF8.GetBytes(data);
    }
}
