using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueGraphWindow : EditorWindow
{
    private int nodeWidth = 180;
    private int nodeHeight = 60;
    private int horizontalSpacing = 40;
    private int verticalSpacing = 40;
    private Vector2 scrollPosition = Vector2.zero;

    private float zoom = 1f;
    private const float zoomMin = 0.5f;
    private const float zoomMax = 2f;

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
            if (string.IsNullOrEmpty(path)) continue;

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

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawGrid();
        DrawToolStrip();

        if (questionArray == null || questionArray.Length == 0)
        {
            EditorGUILayout.LabelField("No questions to display.");
            EditorGUILayout.EndScrollView();
            return;
        }

       
        var nodePositions = DialogueTreeLayout.GenerateTreeLayout(
            questionArray,
            nodeWidth,
            nodeHeight,
            horizontalSpacing,
            verticalSpacing
        );

        
        Handles.BeginGUI();
        Handles.color = Color.white;

        for (int i = 0; i < questionArray.Length; i++)
        {
            Question q = questionArray[i];
            for (int j = 0; j < q.choices.Count; j++)
            {
                var next = q.choices[j].nextDialogue;
                if (next == null) continue;
                if (!nodePositions.ContainsKey(next)) continue;

                Rect parentRect = nodePositions[q];
                Rect childRect = nodePositions[next];

                Vector2 start = new Vector2(parentRect.x + parentRect.width / 2, parentRect.y + parentRect.height);
                Vector2 end = new Vector2(childRect.x + childRect.width / 2, childRect.y);

                
                Handles.DrawBezier(
                    start,
                    end,
                    start + Vector2.up * 50, 
                    end + Vector2.down * 50, 
                    Color.white,
                    null,
                    2f
                );
            }
        }

        Handles.EndGUI();

        // --- DRAW NODES ON TOP ---
        for (int i = 0; i < questionArray.Length; i++)
        {
            Question q = questionArray[i];
            if (!nodePositions.TryGetValue(q, out Rect rect)) continue;

            DrawBox(i, q, rect);
        }

        // --- NODE SELECTION ---
        Event e = Event.current;
        if (e.type == EventType.MouseDown && windowRect.Contains(e.mousePosition))
        {
            Vector2 mouseposition = e.mousePosition;
            for (int i = 0; i < questionArray.Length; i++)
            {
                if (nodeRects[i].Contains(mouseposition))
                {
                    Selection.activeObject = questionArray[i];
                    Debug.Log($"Selected: {questionArray[i].name}");
                    e.Use();
                    break;
                }
                else
                {
                    Selection.activeObject = null;
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawBox(int i, Question q, Rect rect)
    {
        nodeRects[i] = rect;

        Rect outline = new Rect(rect.x, rect.y, rect.width, 7f);

        if (q == Selection.activeObject)
        {
            Rect highlighter = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
            EditorGUI.DrawRect(highlighter, Color.white);
        }

        EditorGUI.DrawRect(rect, new Color(0.25f, 0.25f, 0.25f, 1f));

        bool missingSpeaker = string.IsNullOrEmpty(q.SpeakerName);
        bool missingText = string.IsNullOrEmpty(q.questionText);
        bool missingChoices = (q.choices == null || q.choices.Count == 0);

        if (missingSpeaker && missingText && missingChoices)
        {
            EditorGUI.DrawRect(outline, Color.red);
        }
        else if (missingSpeaker || missingText || missingChoices)
        {
            EditorGUI.DrawRect(outline, Color.orange);
            if (missingSpeaker)
            {
                Rect rectLabel = new Rect(outline.x, outline.y - 43f, rect.width, rect.height);
                EditorGUI.LabelField(rectLabel, "The Speaker Name is empty.");
            }
        }
        else
        {
            EditorGUI.DrawRect(outline, Color.green);
        }

        var centerText = new Rect(rect.x + 10, rect.y + 10, rect.width - 20, rect.height - 20);
        string text = q != null ? q.questionText : "<missing Question asset>";
        EditorGUI.LabelField(centerText, text, TextStyle);
    }
    

    private void DrawGrid(float gridSpacing = 20f, float gridOpacity = 0.1f)
    {
        Color originalColor = Handles.color;
        Handles.color = new Color(1f, 1f, 1f, gridOpacity);

        float width = position.width;
        float height = position.height;

        for (float x = 0; x < width; x += gridSpacing)
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, height, 0));

        for (float y = 0; y < height; y += gridSpacing)
            Handles.DrawLine(new Vector3(0, y, 0), new Vector3(width, y, 0));

        Handles.color = originalColor;
    }

    void DrawToolStrip()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.Label("Zoom", GUILayout.Width(40));
        zoom = GUILayout.HorizontalSlider(zoom, zoomMin, zoomMax, GUILayout.Width(150));
        GUILayout.Label($"{zoom:0.00}x", GUILayout.Width(40));

        GUILayout.EndHorizontal();
    }

    void SearchByName(string text)
    {
        for (int i = 0; i < questionArray.Length; i++)
        {
            if (questionArray[i].SpeakerName == text)
            {
                Debug.Log(questionArray[i].questionText);
                Selection.activeObject = questionArray[i];
            }
        }
    }
}
