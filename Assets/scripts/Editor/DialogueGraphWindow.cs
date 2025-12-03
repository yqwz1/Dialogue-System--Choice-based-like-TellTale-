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
    }

    private void OnGUI()
    {
        
        if (questionArray == null || questionArray.Length == 0)
        {
            EditorGUILayout.LabelField("No questions to display.");
            return;
        }

        for (int i = 0; i < questionArray.Length; i++)
        {
            Question q = questionArray[i];
            int gridx = i % 3;
            int gridy = i / 3;
            Vector2 GF = gridformula(gridx, gridy);
            Rect grid = new Rect(GF.x, GF.y, nodeWidth, nodeHeight);

            EditorGUI.DrawRect(grid, new Color(0.5f, 0.5f, 0.5f, 0.5f));

            string text = (q != null) ? q.questionText : "<missing Question asset>";
            GUI.Label(new Rect(grid.x + 8, grid.y + 8, grid.width - 16, grid.height - 16), text);
        }
    }

    private Vector2 gridformula(int gridX, int gridY)
    {
        int screenX = gridX * (nodeWidth + horizontalSpacing) + marginLeft;
        int screenY = gridY * (nodeHeight + verticalSpacing) + marginTop;
        return new Vector2(screenX, screenY);
    }
}