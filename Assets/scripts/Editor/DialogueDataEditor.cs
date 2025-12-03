using System.IO;
using PlasticGui.WorkspaceWindow.CodeReview.ReviewChanges.Summary;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(Question))]
public class DialogueDataEditor : Editor
{
    SerializedProperty Question;
    private bool showSpeaker = true;  
    private bool showDialogue = true;
    private bool showChoices = true;
    
    private ReorderableList choicesList;
    
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
         
         SerializedProperty choicesProp = serializedObject.FindProperty("choices");
         
         choicesList = new ReorderableList(serializedObject, choicesProp, true, true, true, true);
         
         choicesList.drawHeaderCallback = DrawChoicesHeader;
         choicesList.drawElementCallback = DrawChoiceElement;
         choicesList.elementHeightCallback = GetChoiceElementHeight;
         choicesList.onAddCallback = OnAddChoice;
         choicesList.onRemoveCallback = OnRemoveChoice;
         choicesList.drawElementBackgroundCallback = DrawElementBackground;
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
        SerializedProperty choicesProp = serializedObject.FindProperty("choices");

        if (showChoices)
        {
            choicesList.DoLayoutList();
            
        }
        for (int i = 0; i < choicesProp.arraySize; i++)
        {
            SerializedProperty element = choicesProp.GetArrayElementAtIndex(i).FindPropertyRelative("text");
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
        foreach (Choice answer in question.choices)
        {
            totalChars += answer.text.Length;
        }
        
        return totalChars;
    }

    private void DrawHorizontalLine()
    {
        
        Rect rect = EditorGUILayout.GetControlRect();
    
        EditorGUI.DrawRect(rect, Color.white);
        EditorGUI.DrawRect(rect, Color.gray);
    }
    
    private void DrawChoicesHeader(Rect rect)
    {
       
        int count = choicesList.count;
        EditorGUI.LabelField(rect, $"Choices ({count})", EditorStyles.boldLabel);
    }
    
    private void DrawChoiceElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        
        
        if (isActive)
        {
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.5f, 0.8f, 0.3f));  // Light blue tint
        }
    
        
        SerializedProperty element = choicesList.serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(rect, element, GUIContent.none);
    }
    
    private void OnAddChoice(ReorderableList list)
    {
        
        int newIndex = list.serializedProperty.arraySize;
        list.serializedProperty.arraySize++;
    
      
        SerializedProperty newElement = list.serializedProperty.GetArrayElementAtIndex(newIndex);
    
        
        SerializedProperty textProp = newElement.FindPropertyRelative("text");
        SerializedProperty nextProp = newElement.FindPropertyRelative("nextDialogue");
    
        textProp.stringValue = "New Choice";
        nextProp.objectReferenceValue = null;
    
       
        list.serializedProperty.serializedObject.ApplyModifiedProperties();
    }
    
    private void OnRemoveChoice(ReorderableList list)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
        SerializedProperty textProp = element.FindPropertyRelative("text");
    
        string choiceText = textProp.stringValue;
    
        if (EditorUtility.DisplayDialog(
                "Delete Choice",
                $"Delete choice \"{choiceText}\"?",
                "Delete",
                "Cancel"))
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }
    }
    
    private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
    {
        // Draw custom background
        if (isFocused)
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.4f, 0.7f, 0.2f));
        }
        else if (index % 2 == 0)
        {
            EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.1f));  // Zebra striping
        }
    }
    
    private float GetChoiceElementHeight(int index)
    {
        // Your choice drawer is 2 lines tall + spacing
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;
    
        return (line * 2) + (spacing * 2);
    
        // Plus extra spacing between elements
        return (line * 2) + (spacing * 3);  // Add gap between choices
    }
}
