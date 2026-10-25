using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScenePosition : Component
{
    private GeneralSaveFile saveFile;

    private void Awake()
    {
        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
    }

    public void Save()
    {
        saveFile.SceneSnapshot.SceneName = SceneManager.GetActiveScene().name;
        saveFile.SceneSnapshot.PlayerPosition = transform.position;
        saveFile.SceneSnapshot.HasPlayerPosition = true;
        SaveLoadService.Instance.SaveProgress();
    }

    public Vector3 GetSpawnPosition()
    {
        SceneSnapshot scene = saveFile.SceneSnapshot;

        if (scene.HasPlayerPosition && scene.SceneName == SceneManager.GetActiveScene().name)
            return scene.PlayerPosition;

        return PlayerSpawnPoint.Instance.transform.position;
    }
}
