using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Teleporter)), CanEditMultipleObjects]
public class TeleporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Ensure the serialized object is up to date

        // Display the Teleport field
        EditorGUILayout.PropertyField(serializedObject.FindProperty("teleport"));

        Teleporter teleporter = (Teleporter)target;

        // Check the value of the teleport enum
        if (teleporter.teleport == Teleport.NewScene)
        {
            // If teleport is NewScene, display sceneName field
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneName"));
        }
        else
        {
            // Otherwise, display scene_X and scene_Y fields
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scene_X"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scene_Y"));
        }

        // Apply any changes made in the GUI to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
