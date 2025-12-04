// csharp
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    private int nodeWidth = 180;
    private int nodeHeight = 120;
    private int horizontalSpacing = 40;
    private int verticalSpacing = 40;
    private int marginLeft = 20;
    private int marginTop = 80;
  
    private Question[] questionArray;
    
    private Rect[] nodeRects;
    
    private GUIStyle _textStyle;
    private GUIStyle TextStyle
    {
        get
        {
            if (_textStyle == null)
            {
                _textStyle = new GUIStyle();
                _textStyle.fontSize = 14;
                _textStyle.fontStyle = FontStyle.Bold;
                _textStyle.normal.textColor = Color.white;
               
            }
            return _textStyle;
        }
    }

    [MenuItem("Window/Dialogue Graph")]
    public static void ShowWindow()
    {
        DialogueGraphWindow window = (DialogueGraphWindow)EditorWindow.GetWindow(typeof(DialogueGraphWindow));
        window.titleContent = new GUIContent("Dialogue Graph");
        window.minSize = new Vector2(800, 600);
    }

    private void OnEnable()
    {
        string[] guids = AssetDatabase.FindAssets("t:Question");
        if (guids == null || guids.Length == 0)
        {
            questionArray = Array.Empty<Question>();
            Debug.Log("DialogueGraphWindow: no Question assets found.");
            return;
        }

        questionArray = new Question[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"DialogueGraphWindow: GUID {guids[i]} returned empty path.");
                continue;
            }

            Question q = AssetDatabase.LoadAssetAtPath<Question>(path);
            if (q == null)
            {
                Debug.LogWarning($"DialogueGraphWindow: failed to load Question at path {path}.");
            }
            questionArray[i] = q;
        }
        
        nodeRects = new Rect[questionArray.Length];
    }

    private void OnGUI()
    {
        Rect windowRect = new Rect(0f, 0f, position.width, position.height);
       EditorGUI.DrawRect(windowRect, new Color(0.15f, 0.15f, 0.15f, 0.9f));
       DrawGrid();
        if (questionArray == null || questionArray.Length == 0)
        {
            EditorGUILayout.LabelField("No questions to display.");
            return;
        }

        for (int i = 0; i < questionArray.Length; i++)
        {
            Question q = questionArray[i];
            
            DrawBox(i,q);

        }
        Event e = Event.current;
        if (e.type == EventType.MouseDown && windowRect.Contains(e.mousePosition))
        {
           Vector2 mousepoistion = e.mousePosition;
           for (int i = 0; i < questionArray.Length; i++)
           {
               Selection.activeObject = questionArray[i];
               Debug.Log($"DialogueGraphWindow: mousepoistion {mousepoistion}");
               e.Use();
           }
        }

        if (e.type == EventType.MouseDrag && windowRect.Contains(e.mousePosition))
        {
            
        }
        
        
        Handles.BeginGUI();
        Handles.color = Color.green;
        for (int i = 0; i < questionArray.Length; i++)
        {
            Question q = questionArray[i];
            for (int j = 0; j < q.choices.Length; j++)
            {
                if (q.choices[j].nextDialogue != null)
                {
                    int gridx = i % 3;
                    int gridy = i / 3;
                    Vector2 GF = gridformula(gridx, gridy);
                    Rect grid = new Rect(GF.x, GF.y, nodeWidth, nodeHeight);
                   Vector2 nodecenter = nodeCenter(GF);
                   
                   int targetIndex = -1;
                   for (int f = 0; f < questionArray.Length; f++)
                   {
                       if (questionArray[f] == q.choices[j].nextDialogue) 
                       {
                           targetIndex = f;
                           break;
                       }
                   }

                   if (targetIndex != -1)
                   {
                       int gridx2 = targetIndex % 3;
                       int gridy2 = targetIndex / 3;
                       Vector2 GF2 = gridformula(gridx2, gridy2);
                       Rect grid2 = new Rect(GF2.x, GF2.y, nodeWidth, nodeHeight);
                       Vector2 nodecenter2 = nodeCenter(GF2);

                       Handles.DrawLine(nodecenter, nodecenter2);
                   }
                   
                   
                    
                }
            }
        }
       
        
        Handles.EndGUI();
        
    }            

    private Vector2 gridformula(int gridX, int gridY)
    {
        int screenX = gridX * (nodeWidth + horizontalSpacing) + marginLeft;
        int screenY = gridY * (nodeHeight + verticalSpacing) + marginTop;
        return new Vector2(screenX, screenY);
    }

    private Vector2 nodeCenter(Vector2 topleft)
    {
        float centerX = topleft.x + (nodeWidth / 2);
        float centerY = topleft.y + (nodeHeight / 2);
        
        return new Vector2(centerX, centerY);
    }

    private void DrawBox(int i , Question q)
    {
        int gridx = i % 3;
        int gridy = i / 3;
        Vector2 GF = gridformula(gridx, gridy);
        Rect grid = new Rect(GF.x, GF.y, nodeWidth, nodeHeight);
        Rect outline = new Rect(grid.x, grid.y, nodeWidth, 10f);
        Rect borderBellow = new Rect(grid.x, grid.y + 120, nodeWidth, 5);
        Rect borderRight = new Rect(grid.x + 180, grid.y, 5, nodeHeight + 5);

        EditorGUI.DrawRect(grid, new Color(0.25f, 0.25f, 0.25f, 1f));
        EditorGUI.DrawRect(borderBellow, new Color(0.11f, 0.11f, 0.11f, 0.8f));
        EditorGUI.DrawRect(borderRight, new Color(0.11f, 0.11f, 0.11f, 0.8f));
        bool missingSpeaker = string.IsNullOrEmpty(q.SpeakerName);
        bool missingText = string.IsNullOrEmpty(q.questionText);
        bool missingChoices = (q.choices == null || q.choices.Length == 0);


        if (missingSpeaker && missingText && missingChoices)
        {
            EditorGUI.DrawRect(outline, Color.red);
        }
        else if (missingSpeaker || missingText || missingChoices)
        {
                
            EditorGUI.DrawRect(outline, Color.orange);

            if (missingSpeaker)
            {
                Rect rect = new Rect(outline.x, outline.y - 43f, nodeWidth, nodeHeight);
                EditorGUI.LabelField(rect, "The Speaker Name is empty.");
            }
        }
        else
        {
              
            EditorGUI.DrawRect(outline, Color.green);
        }


        Vector2 nodecenter = nodeCenter(GF);
        Rect centerText = new Rect(nodecenter.x, nodecenter.y, nodeWidth, nodeHeight);
        string text = (q != null) ? q.questionText : "<missing Question asset>";
        EditorGUI.LabelField(centerText, text, TextStyle);
    }
    
    private void DrawGrid(float gridSpacing = 20f, float gridOpacity = 0.1f)
    {
        Color originalColor = Handles.color;
        Handles.color = new Color(1f, 1f, 1f, gridOpacity);
        
        float width = position.width;
        float height = position.height;
        
        for (float x = 0; x < width; x += gridSpacing)
        {
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, height, 0));
        }
        for (float y = 0; y < height; y += gridSpacing)
        {
            Handles.DrawLine(new Vector3(0, y, 0), new Vector3(width, y, 0));
        }
        Handles.color = originalColor;
    }
}