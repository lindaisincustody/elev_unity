using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SaveId))]
public class SaveIdDrawer : PropertyDrawer
{
    private const string ValueField = "value";
    private const float ButtonWidth = 72f;
    private const float Spacing = 4f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty value = property.FindPropertyRelative(ValueField);

        Rect fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Spacing, position.height);
        Rect buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth, position.height);

        using (new EditorGUI.DisabledScope(true))
            EditorGUI.PropertyField(fieldRect, value, label);

        if (!GUI.Button(buttonRect, "Generate"))
            return;

        if (!string.IsNullOrEmpty(value.stringValue) && !ConfirmRegenerate())
            return;

        Generate(property);
    }

    private static bool ConfirmRegenerate()
    {
        return EditorUtility.DisplayDialog(
            "Regenerate id?",
            "This id may already be referenced by saved progress. Regenerating it orphans that data.",
            "Regenerate",
            "Cancel");
    }

    private static void Generate(SerializedProperty property)
    {
        foreach (Object target in property.serializedObject.targetObjects)
        {
            SerializedObject serializedTarget = new SerializedObject(target);

            serializedTarget.FindProperty(property.propertyPath)
                .FindPropertyRelative(ValueField)
                .stringValue = IdGenerator.New(target.name);

            serializedTarget.ApplyModifiedProperties();
        }
    }
}
