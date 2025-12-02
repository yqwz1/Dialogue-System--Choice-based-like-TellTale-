using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Question))]
public class DialogueDataEditor : Editor
{
    SerializedProperty Question;
    
    void OnEnable()
    {
         Question = serializedObject.FindProperty("Question");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Speaker", EditorStyles.boldLabel);
        SerializedProperty property = serializedObject.FindProperty("SpeakerName");
        if (property.stringValue == string.Empty)
        {
            
            EditorGUILayout.HelpBox("Speaker name is empty!", MessageType.Error);
        }
        EditorGUILayout.PropertyField(property);

        
        //EditorGUI.DrawRect(new Rect(1,1,400, 0.5f), Color.gray);
        EditorGUILayout.Space(); 


        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
        SerializedProperty property1 = serializedObject.FindProperty("questionText");
        if (property1.stringValue == string.Empty)
        {
            
            EditorGUILayout.HelpBox("questionText is empty!", MessageType.Error);
        }
        EditorGUILayout.PropertyField(property1);

        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Choices", EditorStyles.boldLabel);
        SerializedProperty property2 = serializedObject.FindProperty("answers");
        for (int i = 0; i < property2.arraySize; i++)
        {
            SerializedProperty element = property2.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(element.stringValue))
            {
                EditorGUILayout.HelpBox($"Answer {i} is empty!", MessageType.Warning);
            }
        }
        EditorGUILayout.PropertyField(property2);
        
        
        serializedObject.ApplyModifiedProperties();
        
    }
}
