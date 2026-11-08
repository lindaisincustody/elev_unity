using System;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class SymbolAttempt
{
    public string label;
    public string drawer;
    public Vector2[] points;
}

public static class SymbolRecordings
{
    public static string FilePath => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SymbolRecordings", "attempts.jsonl"));

    public static void Append(SymbolAttempt attempt)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        File.AppendAllText(FilePath, JsonUtility.ToJson(attempt) + "\n");
    }

    public static SymbolAttempt[] Load()
    {
        if (!File.Exists(FilePath))
            return Array.Empty<SymbolAttempt>();

        return File.ReadAllLines(FilePath)
            .Where(line => line.Length > 0)
            .Select(JsonUtility.FromJson<SymbolAttempt>)
            .ToArray();
    }
}
