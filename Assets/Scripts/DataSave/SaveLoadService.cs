using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveLoadService : CoreService
{
    public static SaveLoadService Instance { get; private set; }

    private static readonly Type[] SaveFileTypes = { typeof(GeneralSaveFile) };

    private readonly Dictionary<Type, ISaveFile> saveFilesByType = new Dictionary<Type, ISaveFile>();

    public override UniTask Initialize()
    {
        Instance = this;

        CreateSaveFiles();
        LoadProgress();

        return UniTask.CompletedTask;
    }

    public T Get<T>() where T : class, ISaveFile
    {
        return (T)saveFilesByType[typeof(T)];
    }

    public bool HasSavedProgress()
    {
        foreach (Type type in SaveFileTypes)
        {
            if (File.Exists(PathFor(type)))
                return true;
        }

        return false;
    }

    public void SaveProgress()
    {
        foreach (KeyValuePair<Type, ISaveFile> entry in saveFilesByType)
            File.WriteAllText(PathFor(entry.Key), JsonUtility.ToJson(entry.Value, true));
    }

    public void LoadProgress()
    {
        foreach (Type type in SaveFileTypes)
        {
            string path = PathFor(type);

            if (File.Exists(path))
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), saveFilesByType[type]);
        }
    }

    public void EraseProgress()
    {
        foreach (Type type in SaveFileTypes)
        {
            string path = PathFor(type);

            if (File.Exists(path))
                File.Delete(path);
        }

        CreateSaveFiles();
    }

    private void CreateSaveFiles()
    {
        saveFilesByType.Clear();

        foreach (Type type in SaveFileTypes)
            saveFilesByType[type] = (ISaveFile)Activator.CreateInstance(type);
    }

    private string PathFor(Type type)
    {
        return Path.Combine(Application.persistentDataPath, type.Name + ".json");
    }
}
