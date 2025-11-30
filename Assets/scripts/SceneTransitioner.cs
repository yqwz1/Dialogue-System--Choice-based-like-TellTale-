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
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneByBuildIndex(i).name == nextSceneName)
                {
                    Debug.Log($"SceneTransitioner Scene #{nextSceneName} is found...... Loading Scene");
                    isbuttonClicked = true;
                    SceneManager.LoadScene(nextSceneName);
                }
            }
            isbuttonClicked = true;
          Debug.Log($"[SceneTransitioner] Scene #{nextSceneName} isnt found in BuildSettings");
        }
        else
        {
            isbuttonClicked = true;
            Debug.Log("[SceneTransitioner] NextSceneName is null!");
        }
        
    }
    
}
