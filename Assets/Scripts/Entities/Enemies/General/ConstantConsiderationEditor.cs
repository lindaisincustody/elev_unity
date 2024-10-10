using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Brain))]
public class BrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target object (Brain)
        Brain brain = (Brain)target;

        // Draw the default inspector (so you can still modify other variables)
        DrawDefaultInspector();

        // If the game is playing, calculate and display the utility of each action
        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Action Utilities", EditorStyles.boldLabel);

            foreach (var action in brain.actions)
            {
                float utility = action.CalculateUtility(brain.context);
                EditorGUILayout.LabelField(action.name, utility.ToString());
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Action utilities will be shown in Play mode.", MessageType.Info);
        }
    }
}
