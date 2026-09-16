using UnityEditor;
using UnityEngine;

public static class CustomMenu
{
    [MenuItem("Custom/Reset Data")]
    private static void ResetData()
    {
        if (EditorApplication.isPlaying)
            SaveLoadService.Instance.EraseProgress();
        else
            SaveLoadService.DeleteSaveFiles();

        Debug.Log($"Save data erased from {Application.persistentDataPath}");
    }
}
