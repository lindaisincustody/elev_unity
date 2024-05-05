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
        public LineType lineType;
        public string dialogueLineText;
    }

    public DialogueType dialogueType;
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

[System.Serializable]
public enum DialogueType
{ 
    Dialogue,
    SelfDialogue,
    Narrator,
}

public enum LineType
{
    You,
    Enemy,
    Narrator
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueData))]
public class DialogueDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueData dialogueData = (DialogueData)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueType"));
        if (dialogueData.dialogueType == DialogueType.Dialogue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("otherCharacterName"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Images", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCharacterImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("otherCharacterImage"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dialogue Lines", EditorStyles.boldLabel);
            SerializedProperty textListProperty = serializedObject.FindProperty("textList");
            for (int i = 0; i < textListProperty.arraySize; i++)
            {
                SerializedProperty characterData = textListProperty.GetArrayElementAtIndex(i);
                SerializedProperty dialogueLineTextProperty = characterData.FindPropertyRelative("dialogueLineText");

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Line " + (i + 1)); // Show line number as label
                EditorGUILayout.PropertyField(characterData.FindPropertyRelative("lineType")); // Show isYourText field
                dialogueLineTextProperty.stringValue = EditorGUILayout.TextArea(dialogueLineTextProperty.stringValue, GUILayout.Height(60)); // Larger text area



                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button("Add New Element"))
            {
                textListProperty.arraySize++;
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Remove Last Element"))
            {
                if (textListProperty.arraySize > 0)
                {
                    textListProperty.arraySize--;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Initiate a fight after dialogue", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("activateFight"));
            if (dialogueData.activateFight)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("strengthGameCoinsMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("neutralityGameCoinsMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("intelligenceGameCoinsMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("coordinationGameCoinsMultiplier"));
            }
        }

        else if (dialogueData.dialogueType == DialogueType.SelfDialogue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Images", EditorStyles.boldLabel);
            SerializedProperty textListProperty = serializedObject.FindProperty("textList");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCharacterImage"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Self-Dialogue Lines", EditorStyles.boldLabel);

            for (int i = 0; i < textListProperty.arraySize; i++)
            {
                SerializedProperty characterData = textListProperty.GetArrayElementAtIndex(i);
                SerializedProperty dialogueLineTextProperty = characterData.FindPropertyRelative("dialogueLineText");

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Line " + (i + 1)); // Show line number as label
                dialogueLineTextProperty.stringValue = EditorGUILayout.TextArea(dialogueLineTextProperty.stringValue, GUILayout.Height(60)); // Larger text area
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button("Add New Element"))
            {
                textListProperty.arraySize++;
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Remove Last Element"))
            {
                if (textListProperty.arraySize > 0)
                {
                    textListProperty.arraySize--;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        else if (dialogueData.dialogueType == DialogueType.Narrator)
        {
            SerializedProperty textListProperty = serializedObject.FindProperty("textList");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Narrator Lines", EditorStyles.boldLabel);

            for (int i = 0; i < textListProperty.arraySize; i++)
            {
                SerializedProperty characterData = textListProperty.GetArrayElementAtIndex(i);
                SerializedProperty dialogueLineTextProperty = characterData.FindPropertyRelative("dialogueLineText");

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Line " + (i + 1)); // Show line number as label
                dialogueLineTextProperty.stringValue = EditorGUILayout.TextArea(dialogueLineTextProperty.stringValue, GUILayout.Height(60)); // Larger text area
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button("Add New Element"))
            {
                textListProperty.arraySize++;
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Remove Last Element"))
            {
                if (textListProperty.arraySize > 0)
                {
                    textListProperty.arraySize--;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
      
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
