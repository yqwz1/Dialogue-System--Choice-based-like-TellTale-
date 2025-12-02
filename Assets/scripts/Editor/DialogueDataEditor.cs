using System.IO;
using PlasticGui.WorkspaceWindow.CodeReview.ReviewChanges.Summary;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Question))]
public class DialogueDataEditor : Editor
{
    SerializedProperty Question;
    private bool showSpeaker = true;  
    private bool showDialogue = true;
    private bool showChoices = true;
    
    private GUIStyle _headerStyle;
    private GUIStyle HeaderStyle
    {
        get
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle();
                _headerStyle.fontSize = 20;
                _headerStyle.fontStyle = FontStyle.Bold;
                _headerStyle.normal.textColor = Color.white;
                _headerStyle.margin = new RectOffset(10, 10, 10, 10);
            }
            return _headerStyle;
        }
    }
    
    private GUIStyle _foldoutStyle;
    private GUIStyle FoldoutStyle
    {
        get
        {
            if (_foldoutStyle == null)
            {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout);
                _foldoutStyle.fontStyle = FontStyle.Bold;
                _foldoutStyle.fontSize = 16;
                _foldoutStyle.normal.textColor = Color.white;
            }
            return _foldoutStyle;
        }
    }
   
    
    void OnEnable()
    {
         Question = serializedObject.FindProperty("Question");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        Question question = (Question)target;
        string assetPath = AssetDatabase.GetAssetPath(question);
        string fileName = Path.GetFileName(assetPath);
        
        
        int totalChars = CalculateTotalCharacters(question, fileName);
        
       
        DrawAssetHeader(fileName, totalChars);
        
        
        DrawSpeakerSection();
        DrawDialogueSection();
        DrawChoicesSection();
        
        serializedObject.ApplyModifiedProperties();
        
        
    }

    private void DrawDialogueSection()
    {
        showDialogue = EditorGUILayout.Foldout(showDialogue, "Dialogue", FoldoutStyle);
        EditorGUILayout.Space();
        SerializedProperty dialogueTextProp = serializedObject.FindProperty("questionText");
        if (dialogueTextProp.stringValue == string.Empty)
        {
            EditorGUILayout.Space(); 
            EditorGUILayout.HelpBox("questionText is empty!", MessageType.Error);
            EditorGUILayout.Space(); 
        }

        if (showDialogue)
        {
            EditorGUILayout.PropertyField(dialogueTextProp);
        }
        

        EditorGUILayout.Space();
    }

    private void DrawChoicesSection()
    {
        showChoices = EditorGUILayout.Foldout(showChoices, "Choices", FoldoutStyle);
        EditorGUILayout.Space();
        SerializedProperty choicesProp = serializedObject.FindProperty("answers");

        if (showChoices)
        {
            EditorGUILayout.PropertyField(choicesProp);
        }
        for (int i = 0; i < choicesProp.arraySize; i++)
        {
            SerializedProperty element = choicesProp.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(element.stringValue))
            {
                EditorGUILayout.Space(); 
                EditorGUILayout.HelpBox($"Answer {i} is empty!", MessageType.Warning);
                EditorGUILayout.Space(); 
            }
        }
    }

    private void DrawSpeakerSection()
    {
        showSpeaker = EditorGUILayout.Foldout(showSpeaker, "Speaker", FoldoutStyle);
        EditorGUILayout.Space();
        SerializedProperty speakerNameProp = serializedObject.FindProperty("SpeakerName");
        if (speakerNameProp.stringValue == string.Empty)
        {
            EditorGUILayout.Space(); 
            EditorGUILayout.HelpBox("Speaker name is empty!", MessageType.Error);
            EditorGUILayout.Space(); 
        }
        if (showSpeaker)
        {
            EditorGUILayout.PropertyField(speakerNameProp);
        }

        
      
        EditorGUILayout.Space(); 
    }

    private void DrawAssetHeader(string fileName, int totalChars)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    
        EditorGUILayout.LabelField("DIALOGUE DATA", HeaderStyle);
        EditorGUILayout.LabelField($"File: {fileName}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"Characters: {totalChars}", EditorStyles.miniLabel);
    
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

    }

    private int CalculateTotalCharacters(Question question , string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || question == null)
        {
            return 0;
        }
        int totalChars = question.questionText.Length + fileName.Length;
        foreach (string answer in question.answers)
        {
            totalChars += answer.Length;
        }
        
        return totalChars;
    }

    private void DrawHorizontalLine()
    {
        
        Rect rect = EditorGUILayout.GetControlRect();
    
        EditorGUI.DrawRect(rect, Color.white);
        EditorGUI.DrawRect(rect, Color.gray);
    }
}
