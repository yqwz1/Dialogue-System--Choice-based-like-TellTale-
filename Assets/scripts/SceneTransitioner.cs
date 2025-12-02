using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitioner : MonoBehaviour
{
    public float transitionDelay = 1.5f;
    public string nextSceneName;
    public bool isbuttonClicked = false;

    private void OnEnable()
    {
        ChoiceHandler.OnChoiceMade += HandleChoiceMade;
    }

    private void OnDisable()
    {
        ChoiceHandler.OnChoiceMade -= HandleChoiceMade;
    }

    private void HandleChoiceMade(int index)
    {
        if (!isbuttonClicked) 
        {
            isbuttonClicked = true;
            StartCoroutine(TransitionToNextScene());
        }
    }

    private IEnumerator TransitionToNextScene()
    {
        isbuttonClicked = false;
        Debug.Log("Transitioning is starting");
        yield return new WaitForSeconds(transitionDelay);
        Debug.Log("Transitioning is loading");
        
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        bool sceneFound = false;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
    
            if (sceneName == nextSceneName)
            {
                sceneFound = true;
                break;
            }
        }

        if (sceneFound)
        {
            Debug.Log($"[SceneTransitioner] Loading scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError($"[SceneTransitioner] Scene '{nextSceneName}' not found in Build Settings!");
        }
    }
    
}
