#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class PlayModeRestarter
{
    private const string PendingKey = "PlayModeRestarter.Pending";

    static PlayModeRestarter()
    {
        EditorApplication.update += Update;
    }

    public static void Request()
    {
        SessionState.SetBool(PendingKey, true);
        EditorApplication.isPlaying = false;
    }

    private static void Update()
    {
        if (!SessionState.GetBool(PendingKey, false))
            return;

        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        SessionState.SetBool(PendingKey, false);
        EditorApplication.isPlaying = true;
    }
}
#endif
