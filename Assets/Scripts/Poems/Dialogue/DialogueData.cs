using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Dialogue", menuName = "Custom/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class CharacterData
    {
        public bool isYourText;
        public string dialogueLineText;
    }

    public string otherCharacterName;
    public CharacterData[] textList;
    public Sprite mainCharacterImage;
    public Sprite otherCharacterImage;
    public bool activateFight;

    [Range(1, 2)]
    public float strengthGameCoinsMultiplier = 1;

    [Range(1, 2)]
    public float neutralityGameCoinsMultiplier = 1;

    [Range(1, 2)]
    public float intelligenceGameCoinsMultiplier = 1;

    [Range(1, 2)]
    public float coordinationGameCoinsMultiplier = 1;
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueData))]
public class DialogueDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueData dialogueData = (DialogueData)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("otherCharacterName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textList"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCharacterImage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("otherCharacterImage"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("activateFight"));

        if (dialogueData.activateFight)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("strengthGameCoinsMultiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("neutralityGameCoinsMultiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("intelligenceGameCoinsMultiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("coordinationGameCoinsMultiplier"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
